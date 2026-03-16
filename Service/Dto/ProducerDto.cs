using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dto
{
    public class ProducerDto
    {
        public int UserId {  get; set; }
        public string CompanyName { get; set; }
        public string? Bio { get; set; }
        public UserRegisterDto User { get; set; }
    }
}
