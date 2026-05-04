using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticSystem.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }


        public int? UserId { get; set; } 
        public virtual User User { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
