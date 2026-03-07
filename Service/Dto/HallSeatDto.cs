using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dto
{
    public class HallSeatDto
    {
        public int Id { get; set; }
        public int HallID { get; set; }
        public int RowNumber { get; set; }
        public int SeatNumber { get; set; }
        public double AddPrice { get; set; }
        public string TypeOfPlace { get; set; }
    }
}
