using MangaScraping.Events;
using MangaScraping.Models;
using MangaScraping.Sources;
using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Managers {
    public class MangaHostSourceManager : HqSourceManager<MangaHostSource> {

        public MangaHostSourceManager(CacheManager cacheManager, MangaHostSource mangaHost) : base(cacheManager, mangaHost) {
            UpdatePage = "https://mangashost.com/";
            LibraryPage = "https://mangahosts.com/mangas";
        }

        public override IHqSourceManager Search(string hqTitle, out List<Hq> result) {
            CoreEventHub.OnProcessingStart(this, new ProcessingEventArgs(DateTime.Now));
            result = _hqSource.Search(hqTitle);
            CoreEventHub.OnProcessingEnd(this, new ProcessingEventArgs(DateTime.Now));
            return this;
        }
    }
}
