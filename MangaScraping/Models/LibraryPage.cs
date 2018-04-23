using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Models {
    public class LibraryPage {
        public string Link { get; set; }
        public string FinalizedPage { get; set; }
        public string NextPage { get; set; }
        public List<Hq> Hqs { get; set; }
        public Dictionary<string, string> Letras { get; set; }
    }
}
