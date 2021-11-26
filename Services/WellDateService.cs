using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WellDataService.Dtos.RequestDto;
using WellDataService.Dtos.ResponseDto;
using WellDataService.Models;

namespace WellDataService.Services
{
    public class WellDateService : IWellDateService
    {
        private readonly WellDBContext _wellDBContext;
        private readonly IMapper _mapper;

        public WellDateService(WellDBContext wellDBContext, IMapper mapper)
        {
            _wellDBContext = wellDBContext;
            _mapper = mapper;
        }

        public async Task<DeviceResponseDto> AddDeviceAsync(AddDeviceRequestDto deviceRequestDto)
        {
            var result = new DeviceResponseDto() { data = new DeviceDataDto(), meta = new MetaDto() };
            float originValue;
            if (deviceRequestDto.OriginValue != null && deviceRequestDto.OriginValue != "")
            {
                originValue = float.Parse(deviceRequestDto.OriginValue);
            }
            else
            {
                originValue = 376;
            }
            
            Device dev = new Device { Id = Guid.NewGuid(), Imei = deviceRequestDto.Imei, Name = deviceRequestDto.Name,
                Address = deviceRequestDto.Address,OriginValue = originValue };
            var listdev = await _wellDBContext.devices.Where(d => d.Name == deviceRequestDto.Name).ToListAsync();
            if (listdev.Count != 0)
            {
                result.meta.msg = "设备已存在！";
                result.meta.status = 400;
                return result;
            }
            listdev = await _wellDBContext.devices.Where(d => d.Imei == deviceRequestDto.Imei).ToListAsync();
            if (listdev.Count != 0)
            {
                result.meta.msg = "设备序列号已存在！";
                result.meta.status = 400;
                return result;
            }
            await _wellDBContext.devices.AddAsync(dev);
            if (await _wellDBContext.SaveChangesAsync() >= 0)
            {
                result.data = _mapper.Map<DeviceDataDto>(dev);
                result.meta.msg = "添加成功！";
                result.meta.status = 200;
                return result;
            }
            else
            {
                result.meta.msg = "添加失败！";
                result.meta.status = 400;
                return result;
            }
        }

        public async Task<DelDeviceResponseDto> DelDeviceAsync(Guid id)
        {
            var result = new DelDeviceResponseDto() { meta = new MetaDto() };
            var dev = await _wellDBContext.devices.Where(d => d.Id == id).FirstOrDefaultAsync();
            _wellDBContext.devices.Remove(dev);
            if (_wellDBContext.SaveChanges() > 0)
            {
                result.meta.msg = "删除设备成功！";
                result.meta.status = 200;
                return result;
            }
            result.meta.msg = "删除设备失败！";
            result.meta.status = 400;
            return result;
        }

        public async Task<DeviceResponseDto> UpdateDeviceAsync(UpdateDeviceRequestDto updateDeviceRequestDto)
        {
            var result = new DeviceResponseDto() { data = new DeviceDataDto(), meta = new MetaDto() };
            float originValue;
            if (updateDeviceRequestDto.OriginValue != null && updateDeviceRequestDto.OriginValue != "")
            {
                originValue = float.Parse(updateDeviceRequestDto.OriginValue);
            }
            else
            {
                originValue = 376;
            }
            Device dev = new Device { Id = updateDeviceRequestDto.Id, Imei = updateDeviceRequestDto.Imei, Name = updateDeviceRequestDto.Name, Address = updateDeviceRequestDto.Address,OriginValue = originValue};
            //更新也应该检查重名
            var checkresult = await _wellDBContext.devices.Where(d => d.Name == updateDeviceRequestDto.Name && d.Id != updateDeviceRequestDto.Id).FirstOrDefaultAsync();
            if (checkresult != null)
            {
                result.meta.msg = "更新失败,设备已存在！";
                result.meta.status = 400;
                return result;
            }
            _wellDBContext.devices.Update(dev);
            if (_wellDBContext.SaveChanges() >= 0)
            {
                result.data = _mapper.Map<DeviceDataDto>(dev);
                result.meta.msg = "更新成功！";
                result.meta.status = 200;
                return result;
            }
            else
            {
                result.meta.msg = "更新失败！";
                result.meta.status = 400;
                return result;
            }
        }

        public async Task<DevicesResponseDto> GetDevicesAsync(int page)
        {
            var result = new DevicesResponseDto() { meta = new MetaDto()};
            var data = await _wellDBContext.devices.ToListAsync();
            result.data = data.Skip(10 * (page - 1)).Take(10);
            result.meta.msg = "获取成功！";
            result.meta.status = 200;
            result.count = data.Count();
            return result;
        }

        public async Task AddWellDataAsync(AddDeviceDataRequestDto addDeviceDataRequestDto)
        {
            var dev =await _wellDBContext.devices.Where(d => d.Imei == addDeviceDataRequestDto.Imei).FirstOrDefaultAsync();
            if (dev != null) 
            {
                await _wellDBContext.deviceDatas.AddAsync(new DeviceData()
                {
                    Time = DateTime.Now,
                    WellName = dev.Name,
                    Temperature = addDeviceDataRequestDto.Temperature,
                    OffsetValue = (dev.OriginValue - addDeviceDataRequestDto.OffsetValue).ToString("F2"),
                    IsAlarm = addDeviceDataRequestDto.IsAlarm
                });
                if (await _wellDBContext.SaveChangesAsync() > 0)
                {

                }
                else
                {
                    //写事件
                }
            }                  
        }

        public async Task<DeviceCurrentDataResponseDto> GetCurrentWellDatasAsync(int page)
        {
            List<DeviceDataDataDto> data = new List<DeviceDataDataDto>();
            var result = new DeviceCurrentDataResponseDto() { meta = new MetaDto() };
            var devs = await _wellDBContext.devices.ToListAsync();
            if(devs == null)
            {
                result.meta.msg = "没有设备！";
                result.meta.status = 400;
                return result;
            }
            foreach (var dev in devs)
            {
                var sub = await _wellDBContext.deviceDatas.Where(p => p.WellName == dev.Name).OrderByDescending( d => d.Id).FirstOrDefaultAsync();
                if (sub != null)
                {
                    var subdto = _mapper.Map<DeviceDataDataDto>(sub);
                    subdto.Address = dev.Address;
                    data.Add(subdto);
                }          
            }
            result.data = data.Skip(10 * (page -1)).Take(10);
            result.Count = data.Count;
            result.meta.msg = "获取成功！";
            result.meta.status = 200;
            return result;
        }

        public async Task<HistoryWellDataResponseDto> GetHistoryWellDatasAsync(HistoryWellDatasRequestDto historyWellDatasRequestDto)
        {
            HistoryWellDataResponseDto result = new HistoryWellDataResponseDto() { Option = new OptionDto() };
            var datas = await _wellDBContext.deviceDatas.Where
            (d => d.WellName == historyWellDatasRequestDto.WellName
            && d.Time >= Convert.ToDateTime(historyWellDatasRequestDto.StartTime)
            && d.Time <= Convert.ToDateTime(historyWellDatasRequestDto.StopTime)).ToListAsync();
            var xAxis = datas.Select(d => d.Time).ToArray();
            string[] s = new string[xAxis.Length];
            for (int i = 0; i < xAxis.Length; i++)
            {
                s[i] = xAxis[i].ToString("yyyy/MM/dd hh:mm:ss");
            }
            result.Option.xAxis = s;
            List<seriesDto> listseries = new List<seriesDto>();
            for (int i = 0; i< historyWellDatasRequestDto.Point.Length; i++)
            {
                var series = new seriesDto();
                series.Name = historyWellDatasRequestDto.Point[i];
                //动态列需要表达式目录树
                if (historyWellDatasRequestDto.Point[i] == "温度")
                {
                    var item = datas.Select(d => d.Temperature).ToArray();
                    series.data = item;
                    series.BorderColor = "rgb(255, 70, 144)";
                    series.Color = "rgb(255, 158, 68)";
                    listseries.Add(series);
                }
                if (historyWellDatasRequestDto.Point[i] == "偏移量")
                {
                    var item = datas.Select(d => d.OffsetValue).ToArray();
                    series.data = item;
                    series.BorderColor = "rgb(64,224,205)";
                    series.Color = "rgb(107, 142, 35)";
                    listseries.Add(series);
                }
                if (historyWellDatasRequestDto.Point[i] == "报警")
                {
                    var item = datas.Select(d => d.IsAlarm).ToArray();
                    string[] str = new string[item.Length];
                    for (int j = 0; j < item.Length; j++)
                    {
                        str[j] = item[j]?"1":"0";
                    }
                    series.data = str;
                    series.BorderColor = "rgb(255,0,0)";
                    series.Color = "rgb(255, 99, 71)";
                    listseries.Add(series);
                }
            }
            result.Option.series = listseries;
            return result;
        }
    }
}
