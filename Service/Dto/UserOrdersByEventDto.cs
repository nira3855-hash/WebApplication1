using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Repository.Entities;
namespace Service.Dto
{
    public class UserOrdersByEventDto
    {
      public Event Event { get; set; }
        public List<OrderDetailDto> Seats { get; set; }
    }
}
