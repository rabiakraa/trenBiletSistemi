using Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class SeferMapping : EntityTypeConfiguration<Sefer>
    {
        public SeferMapping()
        {
            ToTable("Seferler");
            HasKey(x => x.SeferID);
            HasRequired(x => x.Tren).WithRequiredPrincipal(x => x.Sefer);
           //eksik
        }
    }
}
