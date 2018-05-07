using MangaScraping.Sources;
using MangaScraping.Models;
using System;
using System.Collections.Generic;
using System.Text;
using MangaScraping.Events;

namespace MangaScraping.Managers {
    public class MangasProjectSourceManager : HqSourceManager<MangasProjectSource> {
        public MangasProjectSourceManager(CacheManager cacheManager, MangasProjectSource hqSource) : base(cacheManager, hqSource) {
            UpdatePage = "https://leitor.net";
            LibraryPage = "https://leitor.net/lista-de-mangas/ordenar-por-nome/todos";
        }

        public override IHqSourceManager Search(string hqTitle, out List<Hq> result) {
            CoreEventHub.OnProcessingStart(this, new ProcessingEventArgs(DateTime.Now));
            result = _hqSource.Search(UpdatePage, hqTitle);
            CoreEventHub.OnProcessingEnd(this, new ProcessingEventArgs(DateTime.Now));
            return this;
        }
    }
}
