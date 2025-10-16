using DAL;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Reflection.Metadata.Ecma335;

namespace ConsoleApp
{
    internal class ConcurrencyCheck
    {

        public static void Run(DbContextOptionsBuilder<Context> config)
        {
            config.LogTo(Console.WriteLine);
            using Context context = new Context(config.Options);

            Order order = new Order { Name = "Zamówienie 1" };
            var product = new Product { Name = "Produkt 1" };
            order.Products.Add(product);

            context.Add(order);
            context.SaveChanges();

            //zmiana produktu w innym kontekście - symulacja innego użytkownika lub procesu
            ChangeOrder(config);
            
            ConcurrencyToken(context, order);
            RowVersion(context, product);

            ConflictResolve(config, order);
        }


        private static void ConflictResolve(DbContextOptionsBuilder<Context> config, Order order)
        {
            using var context = new Context(config.Options);
            var product = new Product { Name = "Produkt 2", Price = 50, Order = order };
            context.Attach(order);
            context.Add(product);
            context.SaveChanges();

            product.Price = product.Price * 1.1m;

            ChangePrice(product.Id, config);

            bool saved = false;
            while(!saved) {
                try
                {
                    context.SaveChanges();
                    saved = true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        //wartości jakie chcemy wprowadzić do bazy danych
                        var currentValues = entry.CurrentValues;
                        //wartości jakie mamy w kontekście (jakie pobraliśmy)
                        var originalValues = entry.OriginalValues;
                        //wartości jakie mamy w bazie danych
                        var databaseValues = entry.GetDatabaseValues();

                        switch (entry.Entity)
                        {
                            case Product p:

                                var currentPrice = (decimal)currentValues[nameof(Product.Price)];
                                var originalValue = (decimal)originalValues[nameof(Product.Price)];
                                var databasePrice = (decimal)databaseValues[nameof(Product.Price)];

                                currentPrice = databasePrice + (currentPrice - originalValue);

                                currentValues[nameof(Product.Price)] = currentPrice;

                                break;
                        }

                        //ustawiamy wartości oryginalne na wartości z bazy danych, aby uniknąć kolejnego konfliktu konkurencji
                        entry.OriginalValues.SetValues(databaseValues); 
                    }
                }
            }
        }

        private static void ChangePrice(int id, DbContextOptionsBuilder<Context> config)
        {
            using var context = new Context(config.Options);
            var product = context.Set<Product>().First(p => p.Id == id);
            product.Price = product.Price + 10;
            context.SaveChanges();
        }

        private static void ConcurrencyToken(Context context, Order order)
        {
            try
            {
                order.Name = "Zamówienie 1 - powtórnie zmieniony";
                context.SaveChanges();

                order.OrderDate = DateTime.Now;
                context.SaveChanges();
            }
            catch
            {

            }
        }

        private static void RowVersion(Context context, Product product)
        {
            try
            {
                product.Price = 1;
                context.SaveChanges();

                product.Name = "Produkt 1 - powtórnie zmieniony";
                context.SaveChanges();
            }
            catch
            {

            }
        }

        private static void ChangeOrder(DbContextOptionsBuilder<Context> config)
        {
            using Context taskContext = new Context(config.Options);
            var product = taskContext.Set<Product>().First();
            product.Name = $"{product.Name} - zmieniony";
            //product.Price = product.Price + 100;

            var order = taskContext.Set<Order>().First();
            order.Name = $"{order.Name} - zmieniony";
            order.OrderDate = DateTime.Now.AddDays(-1);
            taskContext.SaveChanges();
        }
    }
}
