using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Models.MangasProjectApi {
    public class RootObject {
        public List<Series> series { get; set; }
        public List<Release> releases { get; set; }
        public List<MangasProjectApi.Chapter> chapters { get; set; }
        public List<string> images { get; set; }
    }
}
