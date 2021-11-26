using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WellDataService.Dtos.ResponseDto
{
    public class UserResponseDto
    {
        public IEnumerable<UserDataDto> data { get; set;}
        public int count { get; set;}
        public MetaDto meta { get; set;}
    }
}
