using MangaScraping.Databases;
using MangaScraping.Events;
using MangaScraping.Helpers;
using MangaScraping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MangaScraping.Managers {
    public class CacheManager {
        private readonly LibraryContext _libraryContext;

        private static Dictionary<string, object> _cache = new Dictionary<string, object>();

        private readonly object _lockThis = new object();
        private readonly object _lockThis2 = new object();
        private readonly object _lockThis3 = new object();
        private readonly object _lockThis4 = new object();
        private readonly object _lockThis5 = new object();
        private readonly object _lockThis6 = new object();

        public CacheManager(
                LibraryContext libraryContext) {
            _libraryContext = libraryContext;
        }

        public T ModelCache<T>(string url, Func<String, T> method, double timeCache, bool isFinalized, bool withoutCache) where T : ModelBase {
            lock (_lockThis) {
                var model = default(T);

                if (_cache.ContainsKey(url) && !withoutCache) {
                    model = (T)_cache[url];
                    if (InternetChecker.IsConnectedToInternet()) {
                        if ((DateTime.Now - model.TimeInCache).TotalMinutes > timeCache) {
                            model = SaveHqInfo<T>(url, method, isFinalized);
                            if (model == null) {
                                model = (T)_cache[url];
                            }
                        }
                    }
                } else {
                    if (typeof(T).IsAssignableFrom(typeof(Hq))) {
                        if (withoutCache) {
                            model = SaveHqInfo<T>(url, method, isFinalized);
                        } else {
                            if (_libraryContext.Hq.Find().Where(x => x.Link == url).Execute().FirstOrDefault() is Hq hqModel) {
                                hqModel.Chapters = _libraryContext.Chapter.Find().Where(x => x.Hq == hqModel).Execute();
                                model = hqModel as T;
                                _cache[url] = model;
                                if (InternetChecker.IsConnectedToInternet()) {
                                    if (!hqModel.IsDetailedInformation) {
                                        CoreEventHub.OnProcessingProgress(this, new ProcessingEventArgs(DateTime.Now, $"Atualizando Informações"));
                                        model = SaveHqInfo<T>(url, method, isFinalized);
                                        if (model != null) {
                                            _cache[url] = model;
                                        }
                                    } else {
                                        if ((DateTime.Now - hqModel.TimeInCache).TotalMinutes > timeCache) {
                                            CoreEventHub.OnProcessingProgress(this, new ProcessingEventArgs(DateTime.Now, $"Cache Vencido"));
                                            CoreEventHub.OnProcessingProgress(this, new ProcessingEventArgs(DateTime.Now, $"Atualizando..."));
                                            model = SaveHqInfo<T>(url, method, isFinalized);
                                            if (model != null) {
                                                _cache[url] = model;
                                            } else {
                                                model = hqModel as T;
                                                _cache[url] = model;
                                            }
                                        }
                                    }
                                }
                            } else {
                                model = SaveHqInfo<T>(url, method, isFinalized);
                            }
                        }
                    }
                    if (typeof(T).IsAssignableFrom(typeof(Chapter))) {
                        if (withoutCache) {
                            model = method.Invoke(url) as T;
                        } else {
                            if (_libraryContext.Chapter.Find().Where(x => x.Link == url).Execute().FirstOrDefault() is Chapter chapterModel) {
                                chapterModel.Pages = _libraryContext.Page.Find().Where(x => x.Chapter == chapterModel).Execute();
                                if (chapterModel.Pages == null || chapterModel.Pages.Count == 0) {
                                    var chapterFromSite = method.Invoke(url) as Chapter;
                                    chapterModel.Pages = chapterFromSite.Pages;
                                    SavePagesInDb(chapterModel);
                                }
                                model = chapterModel as T;
                            } else {
                                model = method.Invoke(url) as T;
                                SavePagesInDb(model as Chapter);
                            }
                        }
                    }
                }

                return model;
            }
        }

        public T CacheManagement<T>(string url, Func<String, T> method, double timeCache) {
            lock (_lockThis2) {
                var model = default(T);

                if (_cache.ContainsKey(url)) {
                    model = (T)_cache[url];
                } else {
                    CoreEventHub.OnProcessingProgress(this, new ProcessingEventArgs(DateTime.Now, $"Buscando em DB"));
                    if (_libraryContext.Cache.Find().Where(x => x.Link == url).Execute().FirstOrDefault() is Cache cache) {
                        CoreEventHub.OnProcessingProgress(this, new ProcessingEventArgs(DateTime.Now, $"Encontrado!"));
                        if (!InternetChecker.IsConnectedToInternet()) {
                            model = cache.ModelsCache.ToObject<T>();
                        } else {
                            if ((DateTime.Now - cache.Date).TotalMinutes < timeCache) {
                                model = cache.ModelsCache.ToObject<T>();
                            } else {
                                CoreEventHub.OnProcessingProgress(this, new ProcessingEventArgs(DateTime.Now, $"Cache Vencido"));
                                CoreEventHub.OnProcessingProgress(this, new ProcessingEventArgs(DateTime.Now, $"Atualizando..."));
                                model = method.Invoke(url);
                                if (model != null) {
                                    _cache[url] = model;
                                    _libraryContext.Cache.Update(x => new { x.ModelsCache, x.Date }, model.ToBytes(), DateTime.Now)
                                                                             .Where(x => x.Link == url).Execute();
                                } else {
                                    model = cache.ModelsCache.ToObject<T>();
                                    _cache[url] = model;
                                }
                            }
                        }
                    } else {
                        CoreEventHub.OnProcessingProgress(this, new ProcessingEventArgs(DateTime.Now, $"Não encontrado!"));
                        if (InternetChecker.IsConnectedToInternet()) {
                            model = method.Invoke(url);
                            if (model != null) {
                                CoreEventHub.OnProcessingProgress(this, new ProcessingEventArgs(DateTime.Now, $"Criando Cache"));
                                _cache[url] = model;
                                var updt = new Cache {
                                    Link = url, Date = DateTime.Now, ModelsCache = model.ToBytes()
                                };
                                _libraryContext.Cache.Save(updt);
                            }
                        }
                    }
                }

                return model;
            }
        }

        private T SaveHqInfo<T>(string url, Func<String, T> method, bool isFinalized) where T : ModelBase {
            lock (_lockThis3) {
                var hq = new Hq();
                hq = method.Invoke(url) as Hq;
                hq.TimeInCache = DateTime.Now;
                hq.IsDetailedInformation = true;
                if (isFinalized) {
                    hq.TimeInCache = new DateTime(2100, 1, 1);
                }
                _libraryContext.Hq.Save(hq);
                var hqId = _libraryContext.Hq.Find().Where(x => x.Link == url).Execute().FirstOrDefault()?.Id;
                hq.Id = Convert.ToInt32(hqId);
                SaveChaptersInDb(hq);
                hq.Chapters = _libraryContext.Chapter.Find().Where(x => x.Hq == hq).Execute();
                _cache[url] = hq;
                return hq as T;
            }
        }

        private void SaveChaptersInDb(Hq hq) {
            lock (_lockThis4) {
                var chaptersInDb = _libraryContext.Chapter.Find().Where(x => x.Hq == hq).Execute();
                if (chaptersInDb == null) {
                    chaptersInDb = new List<Chapter>();
                }
                var newChapters = new List<Chapter>();
                if (hq.Chapters != null && hq.Chapters.Count > 0) {
                    foreach (var chap in hq.Chapters) {
                        chap.Hq = hq;
                        if (!ContainsChapterIn(chaptersInDb, chap)) {
                            newChapters.Add(chap);
                        }
                    }

                    if (newChapters.Count() > 0) {
                        _libraryContext.Chapter.Save(newChapters);
                    }
                }
            }
        }

        private void SavePagesInDb(Chapter chapter) {
            lock (_lockThis5) {
                var pages = _libraryContext.Page.Find().Where(x => x.Chapter == chapter).Execute();
                if (pages == null || pages.Count == 0) {
                    foreach (var pg in chapter.Pages) {
                        pg.Chapter = chapter;
                    }

                    _libraryContext.Page.Save(chapter.Pages);
                }
            }
        }

        private bool ContainsChapterIn(IEnumerable<Chapter> chapters, Chapter chapter) {
            var conatins = false;
            foreach (var chap in chapters) {
                if (chap.Link == chapter.Link) {
                    conatins = true;
                    break;
                }
            }
            return conatins;
        }
    }
}
