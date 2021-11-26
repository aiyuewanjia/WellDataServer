using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WellDataService.Dtos.ResponseDto
{
    public class OptionDto
    {
        public string[] xAxis {  get; set; }
        public IEnumerable<seriesDto> series { get; set; }
    }
}
