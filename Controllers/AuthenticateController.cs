using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WellDataService.Dtos.RequestDto;
using WellDataService.Dtos.ResponseDto;
using WellDataService.Models;
using WellDataService.Services;

namespace WellDataService.Controllers
{
    public class AuthenticateController : Controller
    {
        private readonly IAuthenticateService _authenticateService;

        public AuthenticateController(IAuthenticateService authenticateService)
        {
            _authenticateService = authenticateService;
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> LoginAsync([FromBody] LoginRequestDto loginRequestDto)
        {
            var result = await _authenticateService.LoginAsync(loginRequestDto);
            return Json(result);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> RegisterAsync([FromBody] RegisterRequestDto registerRequestDto)
        {
            var result = await _authenticateService.RegisterAsync(registerRequestDto);
            return Json(result);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetRolesAsync()
        {
            var result = await _authenticateService.GetRolesAsync();
            return Json(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetUsersAsync(int page)
        {
            var result = await _authenticateService.GetUsersAsync(page);
            return Json(result);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> ChangePasswordAsync([FromBody]ChangePasswordRequestDto changePasswordRequestDto)
        {
            var result = await _authenticateService.ChangePasswordAsync(changePasswordRequestDto);
            return Json(result);
        }
        
        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> ChangePasswordAndRoleAsync([FromBody]ChangePasswordRequestDto changePasswordRequestDto)
        {
            var result = await _authenticateService.ChangePasswordAndRoleAsync(changePasswordRequestDto);
            return Json(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> DelUserAsync(string User)
        {
            return Json(await _authenticateService.DelUserAsync(User));
        }
    }
}
