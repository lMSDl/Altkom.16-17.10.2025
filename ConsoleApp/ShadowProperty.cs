using DAL;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class ShadowProperty
    {
        public static void Run(DbContextOptionsBuilder<Context> config)
        {
            config.LogTo(Console.WriteLine);

            using var context = new Context(config.Options);

            for (int i = 1; i <= 13; i++)
            {
                var order = new Order() { Name = $"Zamówienie {i + 1}" };
                var orderProduct = new Product() { Name = $"Produkt {i + 1}" };
                order.Products.Add(orderProduct);
                context.Add(order);
            }

            context.SaveChanges();
            context.ChangeTracker.Clear();


            var product = context.Set<Product>().Skip(Random.Shared.Next(12)).First();

            //nie moeżmy pobrać Id, ponieważ Order jest null
            //var orderId = product.Order.Id;

            //pobieramy wartość ShadowProperty z kontekstu
            var orderId = context.Entry(product).Property<int>("OrderId").CurrentValue;
            Console.WriteLine(orderId);


            var createdAt = context.Entry(product).Property<DateTime>("CreatedAt").CurrentValue;
            Console.WriteLine(createdAt);

            //ustawiamy wartość ShadowProperty
            context.Entry(product).Property<int>("OrderId").CurrentValue = 3;
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //wyszukujemy produkty z OrderId = 3 używając ShadowProperty w zapytaniu
            var products = context.Set<Product>().Where(x => EF.Property<int>(x, "OrderId") == 3).ToList();
        }
    }
}
