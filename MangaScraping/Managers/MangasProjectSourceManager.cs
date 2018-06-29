using MangaScraping.Sources;
using MangaScraping.Models;
using System;
using System.Collections.Generic;
using System.Text;
using MangaScraping.Events;

namespace MangaScraping.Managers {
    public class MangasProjectSourceManager : HqSourceManager<MangasProjectSource> {
        public MangasProjectSourceManager(string homeLink, string libraryLink, CacheManager cacheManager, MangasProjectSource hqSource) : base(homeLink, libraryLink, cacheManager, hqSource) {
        }

        public override IHqSourceManager Search(string hqTitle, out List<Hq> result) {
            CoreEventHub.OnProcessingStart(this, new ProcessingEventArgs(DateTime.Now));
            result = _hqSource.Search(UpdatePage, hqTitle);
            CoreEventHub.OnProcessingEnd(this, new ProcessingEventArgs(DateTime.Now));
            return this;
        }
    }
}
