using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class Vagon
    {
        public int VagonID { get; set; }
        public int VagonTipiID { get; set; }

        public virtual Tren Tren { get; set; }
        public virtual VagonTipi VagonTipi { get; set; }
        public virtual List<Koltuk> Koltuklar { get; set; }
    }
}
