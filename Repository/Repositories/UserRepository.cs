using Repository.Entities;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class UserRepository : IRepository<User>
    {
            private readonly IContext _context;
            public UserRepository(IContext context)
            {
                this._context = context;
            }
            public User AddItem(User item)
            {
                _context.Users.Add(item);

                _context.save();
                return item;
            }

            public void DeleteItem(int id)
            {
                _context.Users.Remove(GetById(id));
                _context.save();
            }
            public  User GetByEmail(string email)
        {
            return _context.Users.FirstOrDefault(x => x.email == email);
        }
        public List<User> GetAll()
            {
                return _context.Users.ToList();
            }

            public User GetById(int id)
            {
                return _context.Users.FirstOrDefault(x => x.Id == id);
            }

            public User UpdateItem(int id, User item)
            {
            var User = GetById(id);
               User.Name = item.Name;
               User.email = item.email;
               User.password = item.password;
               User.UserRole= item.UserRole;
              _context.save();
              return User;
            }
        
    }
}
