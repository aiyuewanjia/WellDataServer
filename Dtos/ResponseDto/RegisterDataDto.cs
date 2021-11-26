using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WellDataService.Dtos.ResponseDto
{
    public class RegisterDataDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string role { get; set; }
    }
}
