using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Models.MangasProjectApi {
    public class Series {
        public int id_serie { get; set; }
        public string name { get; set; }
        public string label { get; set; }
        public string score { get; set; }
        public string value { get; set; }
        public string author { get; set; }
        public string artist { get; set; }
        public string cover { get; set; }
        public string link { get; set; }
        public bool is_complete { get; set; }
    }
}
