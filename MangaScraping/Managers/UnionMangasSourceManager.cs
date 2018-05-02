using MangaScraping.Sources;
using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Managers {
    public class UnionMangasSourceManager : HqSourceManager<UnionMangasSource> {
        public UnionMangasSourceManager(CacheManager cacheManager, UnionMangasSource hqSource) : base(cacheManager, hqSource) {
            UpdatePage = "http://unionmangas.cc/";
            LibraryPage = "http://unionmangas.cc/mangas";
        }
    }
}
