using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class HallSeat
    {

        public int Id { get; set; }
        [ForeignKey("Hall")]
        public int 	HallID  { get; set; }
       
        public int RowNumber { get; set; }
        public int SeatNumber { get; set; }
        public double AddPrice { get; set; }
        public  string TypeOfPlace { get; set; }
        
    
    }
}
