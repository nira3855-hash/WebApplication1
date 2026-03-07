using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Service.Dto
{
    public class EventDto
    {
        public int Id { get; set; }
        public int ProducerID { get; set; }
        public string Title { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; }
        public double BasePrice { get; set; }
        public string Describe { get; set; }
       
        public int HallID { get; set; }

        public byte[]? Image { get; set; } //התמונה כמחרוזת

    }
}
