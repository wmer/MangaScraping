using MangaScraping.Databases;
using MangaScraping.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Sources {
    public class HqUltimateSource : HqSource {
        public HqUltimateSource(LibraryContext libraryContext, HtmlSourceHelper htmlHelper, BrowserHelper browserHelper) : base(libraryContext, htmlHelper, browserHelper) {
        }
    }
}