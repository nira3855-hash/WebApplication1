using Repository.Entities;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class HallSeatRepository:IRepository<HallSeat>
    {
        private readonly IContext _context;
        public HallSeatRepository(IContext context)
        {
            this._context = context;
        }
        public HallSeat AddItem(HallSeat item)
        {
            _context.HallSeats.Add(item);
    
            _context.save();
            return item;
        }

        public void DeleteItem(int id)
        {
            _context.HallSeats.Remove(GetById(id));
            _context.save();
        }

        public List<HallSeat> GetAll()
        {
            return _context.HallSeats.ToList();
        }

        public HallSeat GetById(int id)
        {
            return _context.HallSeats.FirstOrDefault(x => x.Id == id);
        }

        public HallSeat UpdateItem(int id, HallSeat item)
        {
            var HallSeat = GetById(id);
            HallSeat.HallID = item.HallID;
            HallSeat.RowNumber = item.RowNumber;
            HallSeat.SeatNumber = item.SeatNumber;
            HallSeat.AddPrice = item.AddPrice;
            HallSeat.TypeOfPlace = item.TypeOfPlace;
            _context.save();
            return HallSeat;
        }
    }
}
