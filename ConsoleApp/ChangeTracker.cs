using DAL;
using Microsoft.EntityFrameworkCore;
using Models;

namespace ConsoleApp
{
    internal class ChangeTracker
    {
        public static void Run(DbContextOptions<Context> config)
        {
            using (var context = new Context(config))
            {

                //domylne ustawienie wykrywania zmian w kontekście
                //context.ChangeTracker.AutoDetectChangesEnabled = true;
                //AutoDetectChangesEnabled dziala w przypadku wywołania SaveChanges, Entry, Local

                Order order = new Order()
                {
                    Name = "Zamówienie 1"
                };
                order.Products.Add(
                        new Product() { Name = "Produkt 1" });
                order.Products.Add(
                        new Product() { Name = "Produkt 2" });

                Console.WriteLine(context.ChangeTracker.DebugView.ShortView);

                Console.WriteLine("Order przed dodaniem do kontekstu: " + context.Entry(order).State);
                Console.WriteLine("Product przed dodaniem do kontekstu: " + context.Entry(order.Products.First()).State);

                //context.Attach(order); //przypisuje Order do kontekstu i w zależności od wartości klucza głównego ustawia stan na Unchanged lub Added
                context.Add(order); //wymusza dodanie do kontekstu, nawet jeśli Order jest już w kontekście (stan Added)

                Console.WriteLine("Order po dodaniu do kontekstu: " + context.Entry(order).State);
                Console.WriteLine("Product po dodaniu do kontekstu: " + context.Entry(order.Products.First()).State);

                context.SaveChanges();

                Console.WriteLine("Order po zapisie do bazy: " + context.Entry(order).State);
                Console.WriteLine("Product po zapisie do bazy: " + context.Entry(order.Products.First()).State);

                order.OrderDate = DateTime.Now.AddDays(-1);
                var product1 = order.Products.First();
                var product2 = order.Products.Last();

                order.Products.Remove(product1);


                //odwołanie się do Entry - wywołuje DetectChanges, więc nie musimy tego robić ręcznie
                Console.WriteLine("Order po modyfikacji: " + context.Entry(order).State);
                Console.WriteLine("Order.Name po modyfikacji order: " + context.Entry(order).Property(o => o.Name).IsModified);
                Console.WriteLine("Order.OrderDate po modyfikacji order: " + context.Entry(order).Property(o => o.OrderDate).IsModified);
                Console.WriteLine("Order.Products po modyfikacji order: " + context.Entry(order).Collection(o => o.Products).IsModified);

                Console.WriteLine("Product 1: " + context.Entry(product1).State);
                Console.WriteLine("Product 2: " + context.Entry(product2).State);

                context.SaveChanges();

                Console.WriteLine("Product 1: " + context.Entry(product1).State);
                Console.WriteLine("Product 2: " + context.Entry(product2).State);

                Console.WriteLine("----");
                Console.WriteLine(context.ChangeTracker.DebugView.ShortView);

                Console.WriteLine("----");
                Console.WriteLine(context.ChangeTracker.DebugView.LongView);

                order.Products.Add(new Product() { Name = "Produkt 5" });

                order = new Order()
                {
                    Name = "Zamówienie 2"
                };
                order.Products.Add(
                        new Product() { Name = "Produkt 3" });
                order.Products.Add(
                        new Product() { Name = "Produkt 4" });

                context.Add(order);

                context.ChangeTracker.DetectChanges(); //wymuszenie wykrycia zmian w kontekście, aby zmiany były widoczne w Entry

                Console.WriteLine("----");
                Console.WriteLine(context.ChangeTracker.DebugView.LongView);

                context.SaveChanges();

                Console.WriteLine("----");
                Console.WriteLine(context.ChangeTracker.DebugView.LongView);


                order.Name = "Zamówienie 2 - zmienione";
                order.OrderDate = DateTime.Now.AddDays(-2);

                Console.WriteLine("----");
                Console.WriteLine(context.ChangeTracker.DebugView.LongView);

                context.ChangeTracker.DetectChanges(); //DebugView nie wywołuje DetectChanges, więc musimy to zrobić ręcznie

                Console.WriteLine("----");
                Console.WriteLine(context.ChangeTracker.DebugView.LongView);

                //context.Entry(order).State = EntityState.Unchanged;

                //istnieje możliwość wyłączenia modyfikacji poszczególnych właściwości obiektu, np. Name, OrderDate, Products
                context.Entry(order).Property(x => x.OrderDate).IsModified = false;

                order.CreatedDate = DateTime.Now.AddDays(-10); //CreatedDate nie powinien być modyfikowany

                Console.WriteLine("----");
                Console.WriteLine(context.ChangeTracker.DebugView.ShortView);
                context.SaveChanges();

                context.ChangeTracker.Clear(); //odłącza wszystkie śledzone encje od kontekstu

                Console.WriteLine("----");
                Console.WriteLine(context.ChangeTracker.DebugView.ShortView);

            }
        }


        public static void TrackingProxies(DbContextOptionsBuilder<Context> config)
        {
            config.UseChangeTrackingProxies();

            using (var context = new Context(config.Options) { ChangeTracker = { AutoDetectChangesEnabled = false } })
            {

                /*var order = new Order()
    {
                    Name = "Zamówienie 3",
        Products = new List<Product>()
        {
            new Product() { Name = "Produkt 6" },
            new Product() { Name = "Produkt 7" },
        }
    }
                ; */

                var order = context.CreateProxy<Order>();
                order.Name = "Zamówienie 3";
                order.Products.Add(context.CreateProxy<Product>());
                order.Products.Add(context.CreateProxy<Product>());
                order.Products.ElementAt(0).Name = "Produkt 6";
                order.Products.ElementAt(1).Name = "Produkt 7";


                context.Add(order);

                Console.WriteLine("----");
                Console.WriteLine(context.ChangeTracker.DebugView.LongView);

                context.SaveChanges();

                Console.WriteLine("----");
                Console.WriteLine(context.ChangeTracker.DebugView.LongView);

                order.Name = "Zamówienie 3 - zmienione";

                Console.WriteLine(context.Entry(order).State);

                Console.WriteLine("----");
                Console.WriteLine(context.ChangeTracker.DebugView.LongView);

                /*
                context.ChangeTracker.DetectChanges(); //ręczne wywołanie DetectChanges, bo AutoDetectChangesEnabled = false

                Console.WriteLine(context.Entry(order).State);

                Console.WriteLine("----");
                Console.WriteLine(context.ChangeTracker.DebugView.LongView);
                */


            }
        }


        public static void ChangedNotification(DbContextOptions<Context> config)
        {
            using (var context = new Context(config) { ChangeTracker = { AutoDetectChangesEnabled = false } })
            {

                var order = new Order { Name = "Zamówienie 4" };
                order.OrderDate = DateTime.Now.AddDays(-4);
                order.Products.Add(new Product { Name = "Produkt 8" });

                context.Add(order);

                Console.WriteLine("----");
                Console.WriteLine(context.ChangeTracker.DebugView.LongView);

                context.SaveChanges();

                order.OrderDate = DateTime.Now.AddDays(-3);

                Console.WriteLine("----");
                Console.WriteLine(context.ChangeTracker.DebugView.LongView);

                order.Name = "Zamówienie 4 - zmodyfikowane";

                Console.WriteLine("----");
                Console.WriteLine(context.ChangeTracker.DebugView.LongView);


                context.SaveChanges();
            }
        }
    }
}
