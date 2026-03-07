using Repository.Entities;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class HallRepository:IRepository<Hall>
    {
        private readonly IContext _context;
        public HallRepository(IContext context)
        {
            this._context = context;
        }
        public Hall AddItem(Hall item)
        {
            _context.Halls.Add(item);

            _context.save();
            return item;
        }

        public void DeleteItem(int id)
        {
            _context.Halls.Remove(GetById(id));
            _context.save();
        }

        public List<Hall> GetAll()
        {
            return _context.Halls.ToList();
        }

        public Hall GetById(int id)
        {
            return _context.Halls.FirstOrDefault(x => x.Id == id);
        }

        public Hall UpdateItem(int id, Hall item)
        {
            var Hall = GetById(id);
            Hall.shape = item.shape;
            Hall.name = item.name;
            Hall.numOfSeats = item.numOfSeats;
            Hall.location = item.location;

            _context.save();
            return Hall;
        }
    }
}
