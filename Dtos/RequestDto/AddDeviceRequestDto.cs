using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WellDataService.Dtos.RequestDto
{
    public class AddDeviceRequestDto
    {
        public string Imei {  get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string OriginValue { get; set; }
    }
}
