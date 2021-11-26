using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WellDataService.Dtos.ResponseDto
{
    public class DeviceDataDto
    {
        public Guid Id { get; set; }
        public string Imei { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
