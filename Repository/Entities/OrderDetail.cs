using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public enum OrderStatus
    {
        Reserved,
        Sold,
        Cancelled
    }
    public class OrderDetail
    {
        public int Id { get; set; }
        [ForeignKey("User")]
        public int UserID { get; set; }
        [ForeignKey("Event")]
        public int EventID { get; set; }
        [ForeignKey("HallSeat")]
        public int HallSeatID { get; set; }
        public double PriceAtPurchase { get; set; } 
        public OrderStatus Status {  get; set; }
        public DateTime SelectAt {  get; set; }

        //public string? FakeCardNumber { get; set; } // לדוגמה: "**** **** **** 1234"
        //public string? FakeCVV { get; set; }        // לדוגמה: "123"
        //public string? FakeCardHolderName { get; set; } // לדוגמה: "John Doe"
        //public string? FakeExpiry { get; set; }     // לדוגמה: "12/25"
    }
}
