using Repository.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Models {
    public class Update {
        [PrimaryKey(true)]
        public int Id { get; set; }
        public virtual Hq Hq { get; set; }
        public string Source { get; set; }
        public DateTime TimeCache { get; set; }
        public virtual List<Chapter> Chapters { get; set; }
    }
}
