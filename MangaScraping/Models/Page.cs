using ADO.ORM.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Models {
    public class Page {
        [PrimaryKey(true)]
        public int Id { get; set; }
        public String Source { get; set; }
        public int Number { get; set; }
        public virtual Chapter Chapter { get; set; }
    }
}
