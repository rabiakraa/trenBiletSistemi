using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class Tren
    {
        public int TrenID { get; set; }
        public int VagonID { get; set; }

        public virtual Sefer Sefer { get; set; }
        public virtual List<Vagon> Vagonlar { get; set; }
    }
}
