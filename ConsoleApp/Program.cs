

using ConsoleApp;
using DAL;
using Microsoft.EntityFrameworkCore;
using Models;

var config = new DbContextOptionsBuilder<Context>()
    .UseSqlServer("Server=(local);Database=EF;TrustServerCertificate=True;Integrated Security=true");

using (var context = new Context(config.Options))
{
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
}

//ChangeTracker.Run(config.Options);
//ChangeTracker.TrackingProxies(config);
//ChangeTracker.ChangedNotification(config.Options);

//ConcurrencyCheck.Run(config);

//ShadowProperty.Run(config);

//GlobalFilters.Run(config);

Transactions.Run(config);






void OrderBy(bool orderBy, string columnName)
{
    using var context = new Context(config.Options);

    var query = (IQueryable<Product>)context.Set<Product>();
    if (orderBy)
        query = query.OrderByDescending(SelectColumn(columnName));
    else
        query = query.OrderBy(SelectColumn(columnName));

    query.ToList().ForEach(p => Console.WriteLine($"{p.Id}: {p.Name} - {p.Price}"));
}



System.Linq.Expressions.Expression<Func<Product, string>> SelectColumn(string columnName)
{
    return p => EF.Property<string>(p, columnName);
}