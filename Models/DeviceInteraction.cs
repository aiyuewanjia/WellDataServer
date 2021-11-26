using System;
using System.Net;

namespace WellDataService.Models
{
    public class DeviceInteraction
    {
        public EndPoint Endpoint { get; set; }
        public DateTime OnlineTime { get; set; }
        public int InteractionNum { get; set; }
        public float Temperature { get; set; }
        public float OffsetValue { get; set; }
        public bool IsAlarm { get; set; }
    }
}