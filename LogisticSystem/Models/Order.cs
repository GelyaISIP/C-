using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticSystem.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }  // New, Shipped, Completed

        // Навигационные свойства
        public virtual Client Client { get; set; }
        public virtual ICollection<Shipment> Shipments { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }

        public string Info => $"{Id} - {Client?.Name}";
    }
}
