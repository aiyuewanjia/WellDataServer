using Microsoft.AspNetCore.Authorization;
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
    public class WellController : Controller
    {
        private readonly IWellDateService _wellDataService;
        public WellController(IWellDateService wellDateService)
        {
            _wellDataService = wellDateService;
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> AddDeviceAsync([FromBody] AddDeviceRequestDto addDeviceRequestDto)
        {
            var result = await _wellDataService.AddDeviceAsync(addDeviceRequestDto);
            return Json(result); 
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> DelDeviceAsync(Guid id)
        {
            var result = await _wellDataService.DelDeviceAsync(id);
            return Json(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> UpdateDeviceAsync([FromBody] UpdateDeviceRequestDto updateDeviceRequestDto)
        {
            var result = await _wellDataService.UpdateDeviceAsync(updateDeviceRequestDto);
            return Json(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetDevicesAsync(int page)
        {
            var result = await _wellDataService.GetDevicesAsync(page);
            return Json(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetCurrentWellDatasAsync(int page)
        {
            var result = await _wellDataService.GetCurrentWellDatasAsync(page);
            return Json(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task AddDeviceDataAsync([FromBody] AddDeviceDataRequestDto addDeviceDataRequestDto)
        {
            await _wellDataService.AddWellDataAsync(addDeviceDataRequestDto);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<HistoryWellDataResponseDto> GetHistoryWellDatasAsync(string StartTime, string StopTime,string WellName,string Point)
        {
            HistoryWellDatasRequestDto historyWellDatasRequestDto = new HistoryWellDatasRequestDto() { StartTime = StartTime, StopTime= StopTime, WellName = WellName, Point = Point.Split(",")};
            var result = await _wellDataService.GetHistoryWellDatasAsync(historyWellDatasRequestDto);
            return result;
        }
    }
}
