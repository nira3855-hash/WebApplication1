using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dto
{
    public class AuthResponseDto
    { 
            public UserDto User { get; set; }
            public string Token { get; set; }
        
    }
}
