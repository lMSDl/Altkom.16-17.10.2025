using DAL;
using Models;

namespace ConsoleApp
{
    internal class ComputedColumns
    {

        //Computed Columns - kolumny obliczeniowe
        //kolumny, których wartość jest obliczana na podstawie innych kolumn w tabeli
        //wartość kolumny jest obliczana przez bazę danych podczas wstawiania lub aktualizacji rekordu
        //nie można bezpośrednio ustawić wartości kolumny obliczeniowej
        //przykład: kolumna TotalValue w tabeli Orders, która jest obliczana jako Value * (1 + Tax)
        public static void Run(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<DAL.Context> config)
        {
            Transactions.Run(config, false);

            using (var context = new Context(config.Options))
            {
                var order = new Order
                {
                    Name = "Zamówienie z kolumną obliczeniową",
                    Value = 100,
                    Tax = 0.23f
                };

                context.Add(order);
                context.SaveChanges();
            }


            using (var context = new Context(config.Options))
            {
                var order = context.Set<Order>().FirstOrDefault(o => o.Name == "Zamówienie z kolumną obliczeniową");

                Console.WriteLine($"Order Name: {order.Name}, Value: {order.Value}, Tax: {order.Tax}, TotalValue (Computed): {order.TotalValue}");
            }

        }
    }
}
