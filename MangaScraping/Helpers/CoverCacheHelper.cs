using MangaScraping.Configuration;
using MangaScraping.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace MangaScraping.Helpers {
    public class CoverCacheHelper {
        private object _lock1 = new object();
        private object _lock2 = new object();
        private string directory;

        public CoverCacheHelper() {
            this.directory = CoreConfiguration.CacheLocation;
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
        }

        public string CreateCache(Hq hq) {
            lock (_lock1) {
                if (string.IsNullOrEmpty((string)hq.CoverSource)) return hq.CoverSource;
                try {
                    var pageSource = $"{directory}\\{StringHelper.RemoveSpecialCharacters(hq.Title)}{FormatPage(hq.CoverSource)}";
                    if (!File.Exists(pageSource)) {
                        ServicePointManager.DefaultConnectionLimit = 1000;
                        using (var webClient = new HttpClient()) {
                            //webClient.Proxy = null;
                            //webClient.DownloadFile(page.Source, pageSource);
                            using (var response = webClient.GetAsync(hq.CoverSource).Result) {
                                var imageByte = response.Content.ReadAsByteArrayAsync().Result;
                                using (var binaryWriter = new BinaryWriter(new FileStream(pageSource,
                                                                              FileMode.Append, FileAccess.Write))) {
                                    binaryWriter.Write(imageByte);
                                }
                            }
                        }
                    }
                    return pageSource;
                } catch {
                    return hq.CoverSource;
                }
            }
        }

        private string FormatPage(string source) {
            lock (_lock2) {
                var formatPosition = source.LastIndexOf(".");
                var format = source.Substring(formatPosition);
                if (format.Contains("?")) {
                    var posi = format.IndexOf("?");
                    var after = format.Substring(posi);
                    format = format.Replace(after, "");
                }

                if (format.Contains("&")) {
                    var posi = format.IndexOf("&");
                    var after = format.Substring(posi);
                    format = format.Replace(after, "");
                }

                return format;
            }
        }
    }
}
