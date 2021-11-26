using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WellDataService.Dtos.ResponseDto
{
    public class LoginResponseDto
    {
        public LoginDataDto data { get; set; }
        public MetaDto meta { get; set; }
    }
}
