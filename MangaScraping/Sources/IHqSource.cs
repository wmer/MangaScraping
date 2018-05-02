using MangaScraping.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Sources {
    public interface IHqSource {
        Hq GetHqInfo(String link);
        Chapter GetChapterInfo(String link);
        LibraryPage GetLibrary(String linkPage);
        List<Update> GetUpdates(string url);
    }
}
