
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WellDataService.Dtos.ResponseDto
{
    public class seriesDto
    {
        public string Name { get; set; }
        public string BorderColor { get; set; }
        public string Color { get; set; }
        public string[] data { get; set; }
    }
}
