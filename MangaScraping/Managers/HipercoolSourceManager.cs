using MangaScraping.Sources;
using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Managers {
    public class HipercoolSourceManager : HqSourceManager<HipercoolSource> {
        public HipercoolSourceManager(CacheManager cacheManager, HipercoolSource hqSource) : base(cacheManager, hqSource) {
            UpdatePage = "https://hiper.cool/";
            LibraryPage = "https://hiper.cool/";
        }
    }
}
