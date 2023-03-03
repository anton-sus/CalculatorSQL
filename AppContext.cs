using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.SQLite;

namespace CalculatorSQL
{
    class AppContext : DbContext
    {
        public DbSet<Log> Logs { get; set; }
        public AppContext(): base("DefaultConnection") { }
    }
}
