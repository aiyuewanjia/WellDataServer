using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WellDataService.Dtos.RequestDto
{
    public class HistoryWellDatasRequestDto
    {
        public string StartTime { get; set; }
        public string StopTime { get; set; }
        public string WellName { get; set; }
        public string[] Point { get; set; }
    }
}
