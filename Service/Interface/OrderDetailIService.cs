using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Dto;

namespace Service.Interface
{
    public interface OrderDetailIService
    {
            OrderDetailDto CompleteOrderItem(OrderDetailCreateDto item);
            OrderDetailDto AddToCartItem(OrderDetailCreateDto item);
            void UpdateItem(int id, OrderDetailDto item);
            OrderDetailDto GetById(int id);
            List<OrderDetailDto> GetAll();
            void DeleteItem(int id);
    }
}
