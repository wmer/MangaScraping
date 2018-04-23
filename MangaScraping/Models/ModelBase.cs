using Repository.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Models {
    public class ModelBase {
        [PrimaryKey(true)]
        public int Id { get; set; }
        public String Link { get; set; }
        public String Title { get; set; }
        public DateTime TimeInCache { get; set; }
    }
}
