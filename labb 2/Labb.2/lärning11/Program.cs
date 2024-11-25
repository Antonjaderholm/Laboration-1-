// See https://aka.ms/new-console-template for more information
using lärning11.models;
using Microsoft.EntityFrameworkCore;
using System;

namespace lärning11
{
    public class ShopContext : DbContext
    {
        public DbSet<custumers> Customers { get; set; }
        public DbSet<order> Orders { get; set; }
        public DbSet<Order_detail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ShopDB;Trusted_Connection=True;");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new ShopContext())
            {
                // Add a customer
                var customer = new custumers
                {
                    Firstname = "John",
                    Lastname = "Doe",
                    Email = "john@example.com"
                };
                context.Customers.Add(customer);

                // Add a product
                var product = new Product
                {
                    Name = "Test Product",
                    Price = 99.99m
                };
                context.Products.Add(product);

                // Create an order
                var order = new order
                {
                    OrderPlace = DateTime.Now,
                    custumers = customer
                };
                context.Orders.Add(order);

                // Add order detail
                var orderDetail = new Order_detail
                {
                    order = order,
                    Product = product,
                    QUANTITY = 1
                };
                context.OrderDetails.Add(orderDetail);

                context.SaveChanges();

                // Display all customers and their orders
                var customers = context.Customers
                    .Include(c => c.Orders)
                    .ToList();

                foreach (var c in customers)
                {
                    Console.WriteLine($"Customer: {c.Firstname} {c.Lastname}");
                    foreach (var o in c.Orders)
                    {
                        Console.WriteLine($"Order placed: {o.OrderPlace}");
                    }
                }
            }
        }
    }
}
