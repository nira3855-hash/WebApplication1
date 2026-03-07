using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Entities;
using Repository.Interfaces;

namespace Repository.Repositories
{
    public class OrderDetailRepository : IRepository<OrderDetail>
    {
        private readonly IContext _context;
        public OrderDetailRepository(IContext context)
        {
            this._context = context;
        }
        public OrderDetail AddItem(OrderDetail item)
        {
            _context.OrderDetails.Add(item);

            _context.save();
            return item;
        }

        public void DeleteItem(int id)
        {
            _context.OrderDetails.Remove(GetById(id));
            _context.save();
        }

        public List<OrderDetail> GetAll()
        {
            return _context.OrderDetails.ToList();
        }

        public OrderDetail GetById(int id)
        {
            return _context.OrderDetails.FirstOrDefault(x => x.Id == id);
        }

        public OrderDetail UpdateItem(int id, OrderDetail item)
        {
            var OrderDetail = GetById(id);
            OrderDetail.EventID = item.EventID ;
            OrderDetail.UserID = item.UserID;
            OrderDetail.PriceAtPurchase = item.PriceAtPurchase;
            OrderDetail.Status = item.Status;
            OrderDetail.HallSeatID = item.HallSeatID;
            OrderDetail.SelectAt = item.SelectAt;


            _context.save();
            return OrderDetail;
        }
    }
}
