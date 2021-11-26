using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WellDataService.Models;

namespace WellDataService.Dtos.ResponseDto
{
    public class DevicesResponseDto
    {
        public IEnumerable<Device> data { get; set; }
        public int count { get; set; }
        public MetaDto meta { get; set; }
    }
}
