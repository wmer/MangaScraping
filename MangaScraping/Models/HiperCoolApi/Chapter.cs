using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Models.HiperCoolApi {
    public class Chapter {
        public string slug { get; set; }
        public int position { get; set; }
        public DateTime publishied_at { get; set; }
        public List<object> videos { get; set; }
        public string _id { get; set; }
        public string title { get; set; }
        public int images { get; set; }
    }
}
