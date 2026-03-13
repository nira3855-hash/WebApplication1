using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dto
{
    public class AuthResponsePDto
    {
        public ProducerDto Producer { get; set; }
        public string Token { get; set; }
    }
}
