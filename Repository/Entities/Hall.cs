using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class Hall
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string location { get; set; }
        public int numOfSeats { get; set; }
        public string shape {  get; set; } 
    }
}
