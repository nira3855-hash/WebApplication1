using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class ProducerRepository:IRepository<Producer>
    {
        private readonly IContext _context;
        public ProducerRepository(IContext context)
        {
            this._context = context;
        }
        public Producer AddItem(Producer item)
        {
            _context.Producers.Add(item);

            _context.save();
            return item;
        }

        public void DeleteItem(int id)
        {
            _context.Producers.Remove(GetById(id));
            _context.save();
        }

        public List<Producer> GetAll()
        {
            return _context.Producers.Include(p => p.User).ToList();
        }

        public Producer GetById(int id)
        {
            return _context.Producers.Include(p => p.User).FirstOrDefault(x => x.UserId == id);
        }

        public Producer UpdateItem(int id, Producer item)
        {
            var Producer = GetById(id);
            Producer.CompanyName = item.CompanyName;
            Producer.UserId = item.UserId;
            Producer.User = item.User;
            Producer.Bio = item.Bio;
            _context.save();
            return Producer;
        }
    }
}
