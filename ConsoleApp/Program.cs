﻿

using ConsoleApp;
using DAL;
using Microsoft.EntityFrameworkCore;

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

ConcurrencyCheck.Run(config);

