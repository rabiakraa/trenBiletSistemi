using Data;
using DATA2;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Context: DbContext
    {
        public Context()
        {
            Database.Connection.ConnectionString = "server = .; database = TrenBiletDb; uid = sa; pwd = 123";
        }

        public DbSet<Bilet> Personeller { get; set; }
        public DbSet<Durak> Duraklar { get; set; }
        public DbSet<Sefer> Seferler { get; set; }
        public DbSet<Durak> IletisimBilgileri { get; set; }
        public DbSet<Bilet> Personeller { get; set; }
        public DbSet<Durak> IletisimBilgileri { get; set; }
        public DbSet<Bilet> Personeller { get; set; }
        public DbSet<Durak> IletisimBilgileri { get; set; }
        public DbSet<Bilet> Personeller { get; set; }

    }
}
