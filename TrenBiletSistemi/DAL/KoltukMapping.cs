using Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class KoltukMapping:EntityTypeConfiguration<Koltuk>
    {
        public KoltukMapping()
        {
            ToTable("Koltuklar");
            Property(x => x.KoltukNo).IsRequired();
            HasRequired(x => x.Vagon).WithMany(x => x.Koltuklar);
        }

    }
}
