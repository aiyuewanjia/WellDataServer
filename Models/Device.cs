using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WellDataService.Models
{
    public class Device
    {
        public Guid Id { get; set; }
        public string Imei {  get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public float OriginValue { get; set; }
    }
}
