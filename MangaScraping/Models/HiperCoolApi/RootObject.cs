using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Models.HiperCoolApi {
    public class RootObject {
        public string title { get; set; }
        public List<string> alternative_titles { get; set; }
        public string slug { get; set; }
        public int position { get; set; }
        public string _id { get; set; }
        public DateTime publishied_at { get; set; }
        public List<object> videos { get; set; }
        public bool pinned { get; set; }
        public Book _book { get; set; }
        public int? images { get; set; }
        public List<HiperCoolApi.Chapter> chapters { get; set; }
        public string type { get; set; }
        public int __v { get; set; }
        public int revision { get; set; }
    }
}
