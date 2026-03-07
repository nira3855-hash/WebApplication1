using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repository.Entities
{
    public class Producer
    {
        [Key] // המפתח הראשי
        public int UserId { get; set; }

        public string CompanyName { get; set; }
        public string Bio { get; set; }

        // הגדרה ברורה שה-UserId הוא גם המפתח הזר
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
