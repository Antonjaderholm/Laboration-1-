using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lärning11.models
{
    public class custumers
    {
        public int id { get; set; }
        public string Firstname { get; set; } = null!;
        public string Lastname { get; set; } = null!;
        public string? Adress { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public ICollection<order> Orders { get; set; } = null!;
    }
}