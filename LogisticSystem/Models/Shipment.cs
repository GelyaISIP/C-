using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticSystem.Models
{
    public class Shipment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int WarehouseId { get; set; }
        public DateTime ShipmentDate { get; set; }

        public virtual Order Order { get; set; }
        public virtual Warehouse Warehouse { get; set; }
    }
}
