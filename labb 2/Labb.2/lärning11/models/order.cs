using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lärning11.models
{
    public class order
    {
        public int id {  get; set; }
        public DateTime OrderPlace { get; set; }
        public DateTime? OrderFulfilled { get; set; }
        public int CustumerId { get; set; }
        public custumers custumers { get; set; } = null!;
        public ICollection<Order_detail> Order_detail { get; set; } = null!;
    }
}
