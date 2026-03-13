using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Entities;


namespace Service.Interface
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
