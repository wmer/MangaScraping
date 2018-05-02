using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Helpers {
    public class HtmlSourceHelper {
        private Object lockThis = new Object();
        private Object lockThis2 = new Object();

        public IDocument GetSourceCodeFromUrl(String url) {
            lock (lockThis) {
                var config = new AngleSharp.Configuration().WithDefaultLoader();
                var source = BrowsingContext.New(config).OpenAsync(url).Result;
                return source;
            }
        }

        public IDocument GetSourceCodeFromHtml(String html) {
            lock (lockThis2) {
                var parser = new HtmlParser();
                return parser.Parse(html);
            }
        }
    }
}
