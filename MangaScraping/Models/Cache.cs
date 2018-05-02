using ADO.ORM.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Models {
    public class Cache {
        [PrimaryKey(true)]
        public int Id { get; set; }
        public string Link { get; set; }
        public DateTime Date { get; set; }
        public byte[] ModelsCache { get; set; }
    }
}
