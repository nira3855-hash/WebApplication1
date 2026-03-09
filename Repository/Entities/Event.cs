using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class Event
    {
        public int Id {  get; set; }
        [ForeignKey("Producer")]
        public int ProducerID  { get; set; }
        public Producer Producer { get; set; }//לדעתי לא צריך גם וגם
        public string Title {  get; set; }
        public DateTime EventDate {  get; set; }
        public string Location {  get; set; }
        public double BasePrice {  get; set; }
        public string Describe { get; set; }
        [ForeignKey("Hall")]
        public int HallID { get; set; }
        public string ?ImageUrl { get; set; }
    }
}
