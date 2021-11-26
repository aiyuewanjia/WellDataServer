using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WellDataService.Dtos.RequestDto;
using WellDataService.Dtos.ResponseDto;

namespace WellDataService.Services
{
    public interface IAuthenticateService
    {     
        public Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto);
        public Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto registerRequestDto);
        public Task<List<string>> GetRolesAsync();
        public Task<UserResponseDto> GetUsersAsync(int page);
        public Task<ChangePasswordResponseDto> ChangePasswordAsync(ChangePasswordRequestDto changePasswordRequestDto);
        public Task<ChangePasswordAndRoleResponseDto> ChangePasswordAndRoleAsync(ChangePasswordRequestDto changePasswordRequestDto);
        public Task<DelUserResponseDto> DelUserAsync(string User);
        
    }
}
