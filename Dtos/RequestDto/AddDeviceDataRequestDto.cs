using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WellDataService.Dtos.RequestDto
{
    public class AddDeviceDataRequestDto
    {
        public string Imei { get; set; }
        public string Temperature { get; set; }
        public float OffsetValue { get; set; }
        public bool IsAlarm { get; set; }
    }
}
