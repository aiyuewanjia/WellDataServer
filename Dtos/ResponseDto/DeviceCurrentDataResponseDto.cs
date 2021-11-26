using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WellDataService.Dtos.ResponseDto
{
    public class DeviceCurrentDataResponseDto
    {
        public IEnumerable<DeviceDataDataDto> data {  get; set; }
        public int Count { get; set; }
        public MetaDto meta { get; set; }

    }
}
