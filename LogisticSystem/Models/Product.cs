using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        [NotMapped]
        public int TotalQuantity => ProductWarehouse?.Sum(s => s.Quantity) ?? 0;
    }
}
