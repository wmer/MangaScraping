using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Models.MangasProjectApi {
    public class Release {
        public string date { get; set; }
        public string date_created { get; set; }
        public int id_serie { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public string range { get; set; }
        public List<MangasProjectApi.Chapter> chapters { get; set; }
        public string link { get; set; }
    }
}
