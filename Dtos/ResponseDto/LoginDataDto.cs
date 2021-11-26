using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WellDataService.Dtos.ResponseDto
{
    public class LoginDataDto
    {
        public string UserName { get; set; }
        public string UserRole { get; set; }
        public string Token { get; set; }
    }
}
