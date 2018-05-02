using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CSharp;

namespace MangaScraping.Models.MangasProjectApi {
    public class Chapter {
        public int id_serie { get; set; }
        public int id_chapter { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string chapter_name { get; set; }
        public string number { get; set; }
        public string date { get; set; }
        public DateTime date_created { get; set; }
        public Dictionary<String, JsonCategoryDescription> releases { get; set; }
    }
}
