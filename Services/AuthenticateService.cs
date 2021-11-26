using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WellDataService.Dtos.RequestDto;
using WellDataService.Dtos.ResponseDto;
using WellDataService.Models;

namespace WellDataService.Services
{
    public class AuthenticateService : IAuthenticateService
    {

        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly WellDBContext _wellDBContext;

        public AuthenticateService(
            IConfiguration configuration, 
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            WellDBContext wellDBContext)
        {
            _configuration = configuration;
            _userManager = userManager;                
            _signInManager = signInManager;
            _wellDBContext = wellDBContext;
        }

        public async Task<List<string>> GetRolesAsync()
        {
            var result = await _wellDBContext.Roles.Select(r => r.Name).ToListAsync();
            return result;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
        {
            LoginResponseDto result = new LoginResponseDto() 
            { 
                data = new LoginDataDto(), 
                meta = new MetaDto() 
            };
            // 1 验证用户名密码  这里校验实际上用的是邮箱，我把邮箱和用户名设置成一样的
            var loginResult = await _signInManager.PasswordSignInAsync(
                loginRequestDto.Username,
                loginRequestDto.Password,
                false,
                false
            );
            if (!loginResult.Succeeded)
            {
                result.meta.status = 400;
                result.meta.msg = "登录失败！";
                return result;
            }           
            var user = await _userManager.FindByNameAsync(loginRequestDto.Username);

            // 2 创建Jwt
            // header
            var signingAlgorithm = SecurityAlgorithms.HmacSha256;
            // payload
            var claims = new List<Claim>
            {
                // sub
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                //new Claim(ClaimTypes.Role, "Admin")
            };
            var roleNames = await _userManager.GetRolesAsync(user);
            foreach (var roleName in roleNames)
            {
                var roleClaim = new Claim(ClaimTypes.Role, roleName);
                claims.Add(roleClaim);
            }
            // signiture
            var secretByte = Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]);
            var signingKey = new SymmetricSecurityKey(secretByte);
            var signingCredentials = new SigningCredentials(signingKey, signingAlgorithm);

            var token = new JwtSecurityToken(
                issuer: _configuration["Authentication:Issuer"],
                audience: _configuration["Authentication:Audience"],
                claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials
            );

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
            var role = await _userManager.GetRolesAsync(user);
            // rturn
            result.data.UserName = user.UserName;
            result.data.UserRole = role[0];
            result.data.Token = "Bearer " + tokenStr;
            result.meta.status = 200;
            result.meta.msg = "登录成功！";
            return result;
        }

        public async Task<UserResponseDto> GetUsersAsync(int page)
        {
            UserResponseDto result = new UserResponseDto() { data = new List<UserDataDto>(), meta = new MetaDto()};
            List<UserDataDto> list = new List<UserDataDto>();
            var listuser = await _wellDBContext.Users.Select(u => new { u.Id, u.UserName, u.UserRoles }).ToListAsync();
            result.count = listuser.Count;
            var t = listuser.Skip(10 * (page - 1)).Take(10).ToList();
            if (t.Count != 0)
            {
                foreach (var user in t)
                {
                    var roleid = await _wellDBContext.UserRoles.Where(u => u.UserId == user.Id).Select(s => s.RoleId).ToListAsync();
                    var role = await _wellDBContext.Roles.Where(r=>r.Id == roleid[0]).ToListAsync();
                    UserDataDto userDataDto = new UserDataDto() { UserId = user.Id, UserName = user.UserName, UserRole = role[0].Name};
                    list.Add(userDataDto);
                }
                result.data = list;
                result.meta.status = 200;
                result.meta.msg = "获取成功！";
            }
            else
            {
                result.meta.status = 400;
                result.meta.msg = "无数据！";
            }
            return result;
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto registerRequestDto)
        {
            var result = new RegisterResponseDto() 
            { 
                data = new RegisterDataDto(), 
                meta = new MetaDto()
            };
            var user = new ApplicationUser()
            {
                UserName = registerRequestDto.UserName,
                Email = registerRequestDto.UserName
            };

            // 2 hash密码，保存用户  密码要注意有要求
            var CreateResult  = await _userManager.CreateAsync(user, registerRequestDto.Password);
            if (!CreateResult.Succeeded)
            {
                result.meta.msg = CreateResult.Errors.ToString();
                result.meta.status = 400;
                return result;
            }
            //查找是否存在角色组
            var role = await _wellDBContext.Roles.Where(r => r.Name == registerRequestDto.Role).FirstAsync();
            //如果角色不存在跳转回角色列表
            if (role == null)
            {
                //如果角色不存在删除注册用户
                await _userManager.DeleteAsync(user);
                result.meta.msg = "角色不存在！";
                result.meta.status = 400;
                return result;
            }
            //角色需要加入
            var Roleresult = await _userManager.AddToRoleAsync(user, registerRequestDto.Role);
            if (!Roleresult.Succeeded)
            {
                //添加到角色组里
                await _userManager.DeleteAsync(user);
                result.meta.msg = "添加角色失败！";
                result.meta.status = 400;
                return result;
            }
            //返回结果
            result.data.UserName = registerRequestDto.UserName;
            result.data.Password = registerRequestDto.Password;
            result.data.role = registerRequestDto.Role;
            result.meta.msg = "注册成功";
            result.meta.status = 200;
            return result;
        }

        public async Task<ChangePasswordResponseDto> ChangePasswordAsync(ChangePasswordRequestDto changePasswordRequestDto)
        {
            var result = new ChangePasswordResponseDto(){meta = new MetaDto()};
            
            var user = await _userManager.FindByNameAsync(changePasswordRequestDto.UserName);
            var fff = await _userManager.RemovePasswordAsync(user);
            var actionResult = await _userManager.AddPasswordAsync(user, changePasswordRequestDto.NewPassword);
            if (actionResult.Succeeded)
            {
                result.meta.msg = "成功";
                result.meta.status = 200;
                return result;
            }
            else
            {
                result.meta.msg = "失败";
                result.meta.status = 400;
                return result;
            }
        }

        public async Task<ChangePasswordAndRoleResponseDto> ChangePasswordAndRoleAsync(ChangePasswordRequestDto changePasswordRequestDto)
        {
            var result = new ChangePasswordAndRoleResponseDto(){meta = new MetaDto()};
            var user = await _userManager.FindByNameAsync(changePasswordRequestDto.UserName);
            await _userManager.RemovePasswordAsync(user);
            var actionResult = await _userManager.AddPasswordAsync(user, changePasswordRequestDto.NewPassword);
            if (actionResult.Succeeded)
            {
                var userrole = await _wellDBContext.UserRoles.Where(u => u.UserId == user.Id).FirstAsync();
                _wellDBContext.UserRoles.Remove(userrole);
                _wellDBContext.SaveChanges();
                userrole.RoleId = (await _wellDBContext.Roles.Where(r => r.Name == changePasswordRequestDto.Role).FirstAsync()).Id;
                _wellDBContext.UserRoles.Add(userrole);
                if (_wellDBContext.SaveChanges() > 0)
                {
                    result.meta.msg = "成功";
                    result.meta.status = 200;
                    return result;
                }
                else
                {
                    result.meta.msg = "失败";
                    result.meta.status = 400;
                    return result;
                }
            }
            else
            {
                result.meta.msg = "失败";
                result.meta.status = 400;
                return result;
            }
        } 
        //还差这个接口测试
        public async Task<DelUserResponseDto> DelUserAsync(string User)
        {
            var result = new DelUserResponseDto(){meta = new MetaDto()};
            var user = await _userManager.FindByNameAsync(User);
            try
            {
                await _userManager.DeleteAsync(user);
                result.meta.msg = "成功";
                result.meta.status = 200;
                return result;
            }
            catch (Exception e)
            {
                result.meta.msg = "失败";
                result.meta.status = 400;
                return result;
            }
        }
    }
}
