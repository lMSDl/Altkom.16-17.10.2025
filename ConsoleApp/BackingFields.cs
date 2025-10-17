using DAL;
using Microsoft.EntityFrameworkCore;
using Models;

namespace ConsoleApp
{
    internal class BackingFields
    {
        public static void Run(DbContextOptionsBuilder<Context> config)
        {

            using (var context = new Context(config.Options))
            {
                var order = new Order()
                {
                    Name = "Zamówienie 1",
                    Products = new List<Product>()
        {
            new Product() { Name = "Produkt 1", Price = 100 },
            new Product() { Name = "Produkt 2", Price = 200 },
        }
                };

                context.Add(order);
                context.SaveChanges();
            }

            using (var context = new Context(config.Options))
            {
                var order = context.Set<Order>().FirstOrDefault();

                Console.WriteLine(order.Name);
            }
        }
    }
}
