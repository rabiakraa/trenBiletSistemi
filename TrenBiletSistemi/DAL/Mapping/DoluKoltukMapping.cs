using Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DoluKoltukMapping : EntityTypeConfiguration<DoluKoltuk>
    {
        public DoluKoltukMapping()
        {
            ToTable("DoluKoltuklar");
           // HasKey(x => x.KoltukID);

            Property(x => x.KoltukNo).IsRequired();

            HasRequired(x => x.Vagon).WithMany(x => x.Koltuklar).HasForeignKey(x => x.VagonID);


        }

    }
}
