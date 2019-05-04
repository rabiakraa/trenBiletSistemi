using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class DoluKoltuk
    {
        public int KoltukID { get; set; }
        public int KoltukNo { get; set; }
        public int VagonID { get; set; }

        public virtual Vagon Vagon { get; set; }

    }
}
