using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dto
{
    public class BestSeatsForEvent
    {
        public int eventId { get; set; }
        public int count { get; set; }
        public bool preferPremium { get; set; }
    }
}
