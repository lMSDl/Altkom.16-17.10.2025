﻿using DAL;
using Microsoft.EntityFrameworkCore;
using Models;

namespace ConsoleApp
{
    internal class Transactions
    {
        public static void Run(DbContextOptionsBuilder<Context> config, bool randomFail = true)
        {




            var products = Enumerable.Range(100, 50).Select(x => new Product { Name = $"Product {x}", Price = x * (decimal)Random.Shared.NextDouble() }).ToList();
            var orders = Enumerable.Range(1, 5).Select(x => new Order { OrderDate = DateTime.Now.AddDays(-x), Name = $"Zamówienie {x}" }).ToList();

            using var context = new Context(config.Options);
            context.RandomFail = randomFail;


            using var transaction = context.Database.BeginTransaction();

            for (int i = 0; i < orders.Count; i++)
            {
                string savePoint = $"SavePoint_{i + 1}";
                transaction.CreateSavepoint(savePoint);
                try
                {
                    var subproducts = products.Skip(i * 10).Take(10).ToList();
                    foreach (var product in subproducts)
                    {
                        context.Add(product);
                        context.SaveChanges();
                    }

                    var order = orders[i];
                    order.Products = subproducts;
                    context.Add(order);
                    context.SaveChanges();
                }
                catch
                {
                    Console.WriteLine(context.ChangeTracker.DebugView.ShortView);
                    transaction.RollbackToSavepoint(savePoint); //cofa zmiany do ostatniego savepoint
                }
                //czyścimy zmiany w kontekście, aby nie były widoczne w kolejnych iteracjach dla savepoint
                context.ChangeTracker.Clear();
            }

            transaction.Commit(); //zapisuje zmiany w bazie danych



           
        }
    }
}
