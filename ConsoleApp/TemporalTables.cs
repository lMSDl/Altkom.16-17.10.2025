using DAL;
using Microsoft.EntityFrameworkCore;
using Models;

namespace ConsoleApp
{
    internal class TemporalTables
    {
        public static void Run(DbContextOptionsBuilder<Context> config)
        {
            using var context = new Context(config.Options);

            var person = new Person
            {
                FirstName = "Jan",
                LastName = "Kowalski",
            };

            person.Address = new Address
            {
                Street = "Mickiewicza",
                City = "Warszawa",
                ZipCode = "00-001"
            };

            context.Add(person);
            context.SaveChanges();

            var time = DateTime.UtcNow;

            Thread.Sleep(2500);

            person.FirstName = "Janusz";
            context.SaveChanges();

            Thread.Sleep(2500);

            person.Address = new Address
            {
                Street = "Słowackiego",
                City = "Kraków",
                ZipCode = "30-001"
            };

            person.LastName = "Nowak";
            context.SaveChanges();

            Thread.Sleep(2500);

            person.FirstName = "Janek";
            context.SaveChanges();

            context.ChangeTracker.Clear();

            person = context.Set<Person>().First();

            //TemporalAll - dostęp do wszystkich danych
            var data = context.Set<Person>().TemporalAll().Select(x => new { x, FROM = EF.Property<DateTime>(x, "ValidFrom"), TO = EF.Property<DateTime>(x, "ValidTo") }).ToList();

            Console.WriteLine($"Obecny stan: {person.FirstName} {person.LastName}");
            person = context.Set<Person>().TemporalAsOf(DateTime.UtcNow.AddSeconds(-5)).Single();
            Console.WriteLine($"Stan z przed 5 sekund: {person.FirstName} {person.LastName}");

            var people = context.Set<Person>().TemporalBetween(DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow.AddSeconds(-1)).ToArray();

            foreach (var p in people)
            {
                Console.WriteLine($"Stan pomiędzy 5 a 1 sek w przeszłości: {p.FirstName} {p.LastName}");
            }


            person = context.Set<Person>().TemporalAsOf(time).Single();
        }
    }
}
