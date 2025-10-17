using DAL;
using Microsoft.EntityFrameworkCore;
using Models;

namespace ConsoleApp
{
    internal class RelatedData
    {
        public static void Run(DbContextOptionsBuilder<Context> config)
        {
            Transactions.Run(config, false);

            config.LogTo(Console.WriteLine);

            using (var context = new Context(config.Options))
            {
                Console.Clear();

                //Eager loading - ładowanie danych razem z głównym obiektem
                //var products = context.Set<Product>().Include(x => x.Order).ToList();
                //var products = context.Set<Product>().Include(x => x.Order).ThenInclude(x => x.Products).ToList();

                //AsSplitQuery - ładowanie danych w wielu zapytaniach
                var products = context.Set<Product>().AsSplitQuery().Include(x => x.Order).ThenInclude(x => x.Products).ToList();
            }

            using (var context = new Context(config.Options))
            {
                var product = context.Set<Product>().First();
                //Explicit loading - ładowanie danych na żądanie

                context.Entry(product).Reference(x => x.Order).Load();

                if(product.Order is not null)
                    context.Entry(product.Order).Collection(x => x.Products).Load();
            }


            Product lazyProduct;
            //ILazyLoader
            using (var context = new Context(config.Options))
            {
                //lazy loading - ładowanie danych przy pierwszym dostępie do właściwości
                lazyProduct = context.Set<Product>().First();

                Console.WriteLine(lazyProduct.Order.Name);

                context.ChangeTracker.Clear();

                lazyProduct = context.Set<Product>().First();
            }
            Console.WriteLine(lazyProduct.Order.Name);


            /*Order order;            
            config.UseLazyLoadingProxies(); //włączenie lazy loadingu na podstawie proxy
            using (var context = new Context(config.Options))
            {
                //lazy loading - ładowanie danych przy pierwszym dostępie do właściwości


                order = context.Set<Order>().First();

                Console.Clear();

                var products = order.Products;
            }*/



        }
    }
}
