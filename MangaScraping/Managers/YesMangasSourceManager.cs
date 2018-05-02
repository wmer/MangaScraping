using MangaScraping.Sources;
using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Managers {
    public class YesMangasSourceManager : HqSourceManager<YesMangasSource> {
        public YesMangasSourceManager(CacheManager cacheManager, YesMangasSource hqSource) : base(cacheManager, hqSource) {
            UpdatePage = "https://yesmangasbr.com/";
            LibraryPage = "https://yesmangasbr.com/mangas";
        }
    }
}
