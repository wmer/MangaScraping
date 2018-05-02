using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Models.HiperCoolApi {
    public class Book {
        public string _id { get; set; }
        public List<object> alternative_titles { get; set; }
        public string slug { get; set; }
        public int chapters { get; set; }
        public string title { get; set; }
        public string synopsis { get; set; }
        public string type { get; set; }
        public DateTime publishied_at { get; set; }
        public int __v { get; set; }
        public int revision { get; set; }
    }
}
