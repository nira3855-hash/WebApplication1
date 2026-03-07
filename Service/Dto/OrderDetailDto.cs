using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Entities;

namespace Service.Dto
{
    public class OrderDetailDto:OrderDetailCreateDto
    {
        public OrderStatus Status { get; set; }
        public double PriceAtPurchase { get; set; }
        public DateTime SelectAt { get; set; }
    }

}
