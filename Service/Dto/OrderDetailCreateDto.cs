using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dto
{
    public class OrderDetailCreateDto
    {
        public int Id { get; set; }
        public int UserID { get; set; }
        public int EventID { get; set; }
        public int HallSeatID { get; set; }
       
        
    }
}
