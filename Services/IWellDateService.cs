using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WellDataService.Dtos.RequestDto;
using WellDataService.Dtos.ResponseDto;
using WellDataService.Models;

namespace WellDataService.Services
{
    public interface IWellDateService
    {
        public Task<DeviceResponseDto> AddDeviceAsync(AddDeviceRequestDto deviceRequestDto);
        public Task<DelDeviceResponseDto> DelDeviceAsync(Guid id);
        public Task<DeviceResponseDto> UpdateDeviceAsync(UpdateDeviceRequestDto updateDeviceRequestDto);
        public Task<DevicesResponseDto> GetDevicesAsync(int page);
        public Task AddWellDataAsync(AddDeviceDataRequestDto addDeviceDataRequestDto);
        public Task<DeviceCurrentDataResponseDto> GetCurrentWellDatasAsync(int page);
        public Task<HistoryWellDataResponseDto> GetHistoryWellDatasAsync(HistoryWellDatasRequestDto historyWellDatasRequestDto);
    }
}
