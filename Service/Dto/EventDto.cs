using System;
using Microsoft.AspNetCore.Http;
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
        //public int TotalTickets { get; set; }
        //public int SolidTickets { get; set; }
        public int HallID { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile? FileImage { get; set; }
    }
}
