using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WellDataService.Models
{
    public class DeviceData
    {
        [Key]
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public string WellName { get; set; }
        public string Temperature { get; set; }
        public string OffsetValue { get; set; }
        public bool IsAlarm { get; set; }
    }
}
