using MangaScraping.Sources;
using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Managers {
    public class HqUltimateSourceManager : HqSourceManager<HqUltimateSource> {
        public HqUltimateSourceManager(CacheManager cacheManager, HqUltimateSource hqSource) : base(cacheManager, hqSource) {
            UpdatePage = "https://hqultimate.com/";
            LibraryPage = "http://hqultimate.com/hqs";
        }
    }
}
