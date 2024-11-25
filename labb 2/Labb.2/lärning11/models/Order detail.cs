using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lärning11.models
{
    public class Order_detail
    {
        public int ID {  get; set; }
        public int  QUANTITY { get; set; }
        public int PRODUCTID { get; set; }
        public int ORDERID { get; set; }

        public order order { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
