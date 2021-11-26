using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WellDataService.Dtos.ResponseDto
{
    public class DeviceDataDataDto
    {
        public string Time { get; set; }
        public string WellName { get; set; }
        public string Address { get; set; }
        public string Temperature { get; set; }
        public string Offset { get; set; }
        public string OffsetValue { get; set; }
        public bool IsAlarm { get; set; }
    }
}
