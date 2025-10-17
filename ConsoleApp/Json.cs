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
    internal class Json
    {

        public static void Run(DbContextOptionsBuilder<Context> config)
        {
            config.LogTo(Console.WriteLine);

            using var context = new Context(config.Options);

            var person = new Person { Address = new Address { City = "Warszawa", Street = "Polna 1", Coordinates = new Coordinates {  Latitude = 19, Longitude = 51 } }, FirstName = "Jan", LastName = "Kowalski" };

            context.Add(person);

            person = new Person { Address = new Address { City = "Kraków", Street = "Długa 5", Coordinates = new Coordinates { Latitude = 20, Longitude = 50 } }, FirstName = "Anna", LastName = "Nowak" };
            context.Add(person);
            context.SaveChanges();
            context.ChangeTracker.Clear();

            person = context.Set<Person>().Where(x => x.Address.City == "Kraków").First();

            person.Address.ZipCode = "30-001";

            context.SaveChanges();

        }
    }
}
