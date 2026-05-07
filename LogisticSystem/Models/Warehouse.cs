using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticSystem.Models
{
    public class Warehouse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }

        public virtual ICollection<Shipment> Shipments { get; set; }
        public virtual ICollection<ProductWarehouse> ProductWarehouse { get; set; }
    }
}
