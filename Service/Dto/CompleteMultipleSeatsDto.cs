using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dto
{
    public class CompleteMultipleSeatsDto
    {
        public int UserId { get; set; }          // מזהה המשתמש
        public int EventId { get; set; }         // מזהה האירוע
        public List<int> HallSeatIds { get; set; } = new List<int>();  // רשימת המושבים
    }
}
