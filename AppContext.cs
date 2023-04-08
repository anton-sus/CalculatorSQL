using System.Data.Entity;

namespace CalculatorSQL
{
    class AppContext : DbContext
    {
        public DbSet<Log> Logs { get; set; }
        public AppContext() : base("DefaultConnection") { }
    }
}
// Примечание: данная реализация использует Entity Framework, чтобы работать с базой данных в C#.
// Класс DbContext - это основной класс фреймворка, который предоставляет все необходимые методы
// и свойства для работы с базами данных. DbSet - это свойство, которое представляет набор сущностей
// в базе данных. Свойство Logs позволяет работать с таблицей Logs в базе данных.
// Конструктор класса AppContext устанавливает строку подключения к базе данных с именем DefaultConnection.