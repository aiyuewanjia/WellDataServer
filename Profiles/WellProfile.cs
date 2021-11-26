using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WellDataService.Dtos.ResponseDto;
using WellDataService.Models;

namespace WellDataService.Profiles
{
    public class WellProfile : Profile
    {
        public WellProfile()
        {
            CreateMap<Device, DeviceDataDto>();
            CreateMap<DeviceData, DeviceDataDataDto>()
                .AfterMap((src, desc) => desc.Temperature += "℃")
                .AfterMap((src, desc) => desc.Offset += "mm")
                .AfterMap((src, desc) => desc.OffsetValue = (float.Parse(desc.OffsetValue)/3).ToString())
                .ForMember(desc => desc.Offset, opt => opt.MapFrom(src => src.OffsetValue))
                .ForMember(desc => desc.Time, opt => opt.MapFrom(src => src.Time.ToString("yyyy/MM/dd HH:mm:ss")));
        }
    }
}
