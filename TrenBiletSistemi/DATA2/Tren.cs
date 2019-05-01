using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data
{
    public class Tren
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TrenID { get; set; }
        public int VagonID { get; set; }

        public virtual Sefer Sefer { get; set; }
        public virtual List<Vagon> Vagonlar { get; set; }
    }
}
