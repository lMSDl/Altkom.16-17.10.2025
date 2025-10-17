

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

//Transactions.Run(config);

//Transactions.Run(config, false);
//OrderBy(false, "Price");

//RelatedData.Run(config);

TemporalTables.Run(config);

//CompileQuery.Run(config);

//BackingFields.Run(config);

//ComputedColumns.Run(config);

Transactions.Run(config, false);


config.LogTo(Console.WriteLine);
using (var context = new Context(config.Options))
{
    var products = context.Set<Product>()/*.Include(x => x.ProductDetails)*/.ToList();

    context.Set<ProductDetails>().Load();
}


    void OrderBy(bool orderBy, string columnName)
    {
        config.LogTo(Console.WriteLine);
        using var context = new Context(config.Options);

        /*var query = (IQueryable<Product>)context.Set<Product>();
        if (!orderBy)
            query = query.OrderByDescending(SelectColumn(columnName));
        else
            query = query.OrderBy(SelectColumn(columnName));*/

        var query = (IQueryable<Product>)context.Set<Product>().OrderBy(SelectColumn(columnName));
        if (!orderBy)
            query = query.Reverse();

        query.ToList().ForEach(p => Console.WriteLine($"{p.Id}: {p.Name} - {p.Price}"));
    }



System.Linq.Expressions.Expression<Func<Product, string>> SelectColumn(string columnName)
{
    return p => EF.Property<string>(p, columnName);
}