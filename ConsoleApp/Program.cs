

using DAL;
using Microsoft.EntityFrameworkCore;
using Models;

var config = new DbContextOptionsBuilder<Context>()
    .UseSqlServer("Server=(local);Database=EF;TrustServerCertificate=True;Integrated Security=true")
    .Options;

using (var context = new Context(config))
{
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
}


using (var context = new Context(config))
{

    //domylne ustawienie wykrywania zmian w kontekście
    //context.ChangeTracker.AutoDetectChangesEnabled = true;
    //AutoDetectChangesEnabled dziala w przypadku wywołania SaveChanges, Entry, Local

    Order order = new Order()
    {
        Name = "Zamówienie 1",
        Products = new List<Product>()
        {
            new Product() { Name = "Produkt 1" },
            new Product() { Name = "Produkt 2" },
        }
    };

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
        Name = "Zamówienie 2",
        Products = new List<Product>()
        {
            new Product() { Name = "Produkt 3" },
            new Product() { Name = "Produkt 4" },
        }
    };

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