using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogisticSystem.Models
{
    public class Product
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string SKU { get; set; }        // артикул
        public int Quantity { get; set; }

        public virtual ICollection<ProductWarehouse> ProductWarehouse { get; set; }
    }
}
