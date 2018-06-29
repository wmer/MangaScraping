using MangaScraping.Events;
using MangaScraping.Models;
using MangaScraping.Sources;
using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Managers {
    public class MangaHostSourceManager : HqSourceManager<MangaHostSource> {
        public MangaHostSourceManager(string homeLink, string libraryLink, CacheManager cacheManager, MangaHostSource hqSource) : base(homeLink, libraryLink, cacheManager, hqSource) {
        }

        public override IHqSourceManager Search(string hqTitle, out List<Hq> result) {
            CoreEventHub.OnProcessingStart(this, new ProcessingEventArgs(DateTime.Now));
            result = _hqSource.Search(hqTitle);
            CoreEventHub.OnProcessingEnd(this, new ProcessingEventArgs(DateTime.Now));
            return this;
        }
    }
}
