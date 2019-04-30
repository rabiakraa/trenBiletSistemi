using Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class KullaniciTipMapping: EntityTypeConfiguration<KullaniciTip>
    {
        public KullaniciTipMapping()
        {
            ToTable("KullanıcıTipleri");
            Property(x => x.TipAdi).IsRequired();
        }
    }
}
