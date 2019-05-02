using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data
{
    public class Vagon
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VagonID { get; set; }
        public int VagonTipiID { get; set; }

        public virtual List<TrenVagon> VagonunTrenleri { get; set; }
        public virtual VagonTipi VagonTipi { get; set; }
        public virtual List<DoluKoltuk> Koltuklar { get; set; }
    }
}
