using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using lärning11.models;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace lärning11.Data
{
    public class ShopContext : DbContext
    {
        public DbSet<custumers> custumers { get; set; } = null!;
        public DbSet<order> orders { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Order_detail> order_Details { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
        {
            optionsBuilder
                .UseSqlServer(@"Data Source=antonsdator;Database=Pizza;Integrated Security=True;TrustServerCertificate=True;")
                .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }
    }
    
}
