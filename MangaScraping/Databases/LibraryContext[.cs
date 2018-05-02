using MangaScraping.Configuration;
using MangaScraping.Models;
using ADO.ORM;
using ADO.ORM.Core.SqLite;
using ADO.ORM.SqLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Databases {
    public class LibraryContext : SqLiteContext {
        public LibraryContext() : base(CoreConfiguration.DatabaseLocation, "library.db") {
        }

        //public LibraryContext() : base("localhost", 3306, "library", "root", "EWSantanas3120") {
        //}

        public SqLiteRepository<Cache> Cache { get; set; }
        public SqLiteRepository<Update> Update { get; set; }
        public SqLiteRepository<Hq> Hq { get; set; }
        public SqLiteRepository<Chapter> Chapter { get; set; }
        public SqLiteRepository<Page> Page { get; set; }
    }
}
