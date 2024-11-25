using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lärning11.models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        [Column(TypeName = "Decimal(6,2)")]
        public decimal Price { get; set; }
    }
}
