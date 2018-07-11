using AngleSharp.Dom;
using MangaScraping.Databases;
using MangaScraping.Events;
using MangaScraping.Helpers;
using MangaScraping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MangaScraping.Sources {
    public class MangaHostSource : HqSource {

        public MangaHostSource(LibraryContext libraryContext, HtmlSourceHelper htmlHelper, BrowserHelper browserHelper) : base(libraryContext, htmlHelper, browserHelper) {
        }

        public List<Hq> Search(string hqTitle) {
            lock (Lock8) {
                var result = new List<Hq>();
                hqTitle = hqTitle.Replace(" ", "+");
                var linkSearch = $"https://mangahost.cc/find/{hqTitle}";
                OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Pegando dados da página"));
                IDocument source = HtmlHelper.GetSourceCodeFromUrl(linkSearch);

                var hqsEl = source.QuerySelectorAll(".table-search tr");
                OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"{hqsEl.Count()} mangas encontrados!"));
                if (hqsEl.Count() > 0) {
                    foreach (var hqEl in hqsEl) {
                        var title = hqEl.QuerySelector("td h3.entry-title")?.TextContent;
                        if (!string.IsNullOrEmpty(title)) {
                            var sinopse = hqEl.QuerySelector("td div.entry-content")?.TextContent;
                            var img = hqEl.QuerySelector("td a img.manga")?.GetAttribute("src");
                            var link = hqEl.QuerySelector("td h3.entry-title a")?.GetAttribute("href");
                            if (!string.IsNullOrEmpty(link)) {
                                result.Add(new Hq { Link = link, Title = CleanTitle(title), Synopsis = sinopse, CoverSource = img });
                            }
                        }
                    }
                } else {
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Não foi encontrado resultados!"));
                }

                OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Tudo pronto"));
                return result;
            }
        }

        public override List<Update> GetUpdates(string url) {
            lock (Lock7) {
                try {
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Buscando Atualizações..."));
                    var driver = BrowserHelper.GetDriver(url);
                    var pageSource = driver.PageSource;
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Pegando dados da página"));
                    IDocument source = HtmlHelper.GetSourceCodeFromHtml(pageSource);
                    driver.Quit();
                    BrowserHelper.CloseDriver();
                    var updates = new List<Update>();
                    if (source == null) throw new Exception("Ocorreu um erro ao buscar informaçoes da Hq");
                    var updatesEl = source.QuerySelectorAll(".table-lancamentos tr");
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"{updatesEl.Count()} mangas encontrados!"));
                    foreach (var update in updatesEl) {
                        var title = update.QuerySelector("td h3.entry-title")?.TextContent;
                        if (!string.IsNullOrEmpty(title)) {
                            title = CleanTitle(title);
                            var img = update.QuerySelector("td a img.manga")?.GetAttribute("src");
                            var link = update.QuerySelector("td h3.entry-title a")?.GetAttribute("href");
                            var chaptersEl = update.QuerySelectorAll(".chapters a.btn");
                            if (!string.IsNullOrEmpty(link)) {
                                var chapters = new List<Chapter>();
                                foreach (var chap in chaptersEl) {
                                    var chapterTitle = chap.GetAttribute("title");
                                    chapterTitle = CleanTitle(chapterTitle);
                                    var chapterLink = chap.GetAttribute("href");
                                    chapters.Add(new Chapter { Link = chapterLink, Title = chapterTitle });
                                }
                                var up = new Update {
                                    Hq = new Hq { Link = link, Title = CleanTitle(title), CoverSource = img },
                                    Chapters = chapters
                                };
                                updates.Add(up);
                                OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, up.Hq, $"{title} Adicionado"));
                            }
                        }
                    }
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Tudo pronto"));
                    return updates;
                } catch (Exception e) {
                    BrowserHelper.CloseDriver();
                    OnProcessingProgressError(new ProcessingErrorEventArgs(DateTime.Now, url, e));
                    return null;
                }
            }
        }

        public override LibraryPage GetLibrary(String linkPage) {
            lock (Lock6) {
                try {
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Buscando Biblioteca..."));
                    var driver = BrowserHelper.GetDriver(linkPage);
                    var pageSource = driver.PageSource;
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Pegando dados da página"));
                    IDocument source = HtmlHelper.GetSourceCodeFromHtml(pageSource);
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Pegando dados da página"));
                    driver.Quit();
                    BrowserHelper.CloseDriver();

                    var hqs = new List<Hq>();
                    if (source == null) throw new Exception("Ocorreu um erro ao buscar informaçoes da Hq");
                    var hqsEl = source.QuerySelectorAll(".container #page .list .thumbnail");
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"{hqsEl.Count()} mangas encontrados!"));
                    var nextPage = source.QuerySelector("div.wp-pagenavi a.nextpostslink")?.GetAttribute("href");
                    var finalizadosEl = source.QuerySelectorAll(".container #page ul.nav-tabs li a");
                    var finalizedPage = "";
                    foreach (var el in finalizadosEl) {
                        if (el.TextContent.Contains("Completos")) {
                            finalizedPage = el.GetAttribute("href");
                        }
                    }
                    var lethers = source.QuerySelectorAll(".paginador ul.letras li a");
                    var letherLink = new Dictionary<string, string>();
                    foreach (var lether in lethers) {
                        letherLink[lether.TextContent] = lether.GetAttribute("href");
                    }
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Preparando para retornar mangas!"));
                    foreach (var hq in hqsEl) {
                        var title = hq.QuerySelector("h3")?.TextContent;
                        var img = hq.QuerySelector("a img")?.GetAttribute("src");
                        var link = hq.QuerySelector("h3 a")?.GetAttribute("href");
                        if (!string.IsNullOrEmpty(link)) {
                            var manga = new Hq { Link = link, Title = CleanTitle(title), CoverSource = img };
                            if (!hqs.Contains(manga)) {
                                hqs.Add(manga);
                                OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, manga, $"{title} Adicionado"));
                            }
                        }
                    }
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Tudo pronto"));
                    return new LibraryPage {
                        Link = linkPage, Hqs = hqs, NextPage = nextPage,
                        FinalizedPage = finalizedPage, Letras = letherLink
                    };
                } catch (Exception e) {
                    BrowserHelper.CloseDriver();
                    OnProcessingProgressError(new ProcessingErrorEventArgs(DateTime.Now, linkPage, e));
                    return null;
                }
            }
        }

        public override Hq GetHqInfo(string link) {
            lock (Lock1) {
                try {
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Pegando dados da página"));
                    var source = HtmlHelper.GetSourceCodeFromUrl(link);
                    var hqInfo = new Hq();
                    if (source == null) throw new Exception("Ocorreu um erro ao buscar informaçoes da Hq");
                    var title = source.QuerySelector(".title-widget .entry-title")?.TextContent;
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Buscando informações de {title}"));
                    var img = source.QuerySelector("div#page img");
                    var synopsis = source.QuerySelector(".pull-left article")?.TextContent;
                    hqInfo.Title = CleanTitle(title);
                    hqInfo.CoverSource = img?.GetAttribute("src");
                    hqInfo.Synopsis = synopsis?.Replace("\n", "").Trim().Replace("\n", "").Trim().Replace("'", "`").Trim();
                    hqInfo.Link = link; var hqInfos = source.QuerySelectorAll("ul.descricao  li");
                    foreach (var info in hqInfos) {
                        if (info.TextContent.Contains("Autor")) {
                            hqInfo.Author = info.TextContent.Replace("Autor:", "").Trim().Replace("\n", "").Trim().Replace("'", "`").Trim();
                        }
                    }
                    var chapters = GetListChapters(source).Reverse<Chapter>().ToList();
                    hqInfo.Chapters = chapters;

                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Tudo pronto!"));
                    return hqInfo;
                } catch (Exception e) {
                    OnProcessingProgressError(new ProcessingErrorEventArgs(DateTime.Now, link, e));
                    return null;
                }
            }
        }

        public override Chapter GetChapterInfo(string link) {
            lock (Lock2) {
                try {
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Pegando dados da página"));
                    IDocument leitor = HtmlHelper.GetSourceCodeFromUrl(link);
                    var chapter = new Chapter();
                    if (leitor == null) throw new Exception("Ocorreu um erro ao buscar informaçoes da Hq");
                    var chapterTitle = leitor.QuerySelector("div.breadcrumb ul.container  li.active span")?.TextContent;
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Buscando informações de {chapterTitle}"));
                    chapter.Title = CleanTitle(chapterTitle);
                    chapter.Link = link;
                    var pages = GetPageList(leitor);
                    if (pages.Count <= 2) {
                        UsingIe = true;
                        var driver = BrowserHelper.GetDriver(link);
                        var pageSource = driver.PageSource;
                        OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Pegando dados da página"));
                        leitor = HtmlHelper.GetSourceCodeFromHtml(pageSource);
                        pages = GetPageList(leitor);
                        driver.Quit();
                        UsingIe = false;
                    }
                    chapter.Pages = pages;
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Tudo pronto"));
                    return chapter;
                } catch (Exception e) {
                    OnProcessingProgressError(new ProcessingErrorEventArgs(DateTime.Now, link, e));
                    return null;
                }
            }
        }

        public List<Chapter> GetListChapters(IDocument hqSource) {
            lock (Lock3) {
                OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Buscando capitulos"));
                var chapterList = new List<Chapter>();
                var chapters = hqSource.QuerySelectorAll("table a.capitulo");
                if (chapters != null && chapters.Length > 0) {
                    foreach (var chapter in chapters) {
                        var chapterLink = chapter.GetAttribute("href");
                        var chapterTitle = $"{chapter.TextContent}";
                        OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Adicionando {chapterTitle}"));
                        var chap = new Chapter { Link = chapterLink, Title = CleanTitle(chapterTitle) };
                        chapterList.Add(chap);
                    }
                } else {
                    chapters = hqSource.QuerySelectorAll(".list_chapters a");
                    if (chapters == null) return chapterList;
                    foreach (var chapterEl in chapters) {
                        var chapter = HtmlHelper.GetSourceCodeFromHtml(chapterEl.GetAttribute("data-content"));
                        var linkEl = chapter.QuerySelector("a");
                        var chapterLink = linkEl?.GetAttribute("href");
                        var chapterTitle = $"Capitulo - {chapterEl.TextContent}";
                        OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Adicionando {chapterTitle}"));
                        var chap = new Chapter { Link = chapterLink, Title = CleanTitle(chapterTitle) };
                        chapterList.Add(chap);
                    }
                }
                return chapterList;
            }
        }

        public List<Models.Page> GetPageList(IDocument chapterSoure) {
            lock (Lock4) {
                OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Buscando páginas"));
                var pageList = new List<Models.Page>();
                var pages = GetPageListFromScript(chapterSoure);
                var mangaPages = pages.QuerySelectorAll("img");
                if (mangaPages == null) return pageList;
                var num = 1;
                foreach (var img in mangaPages) {
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Adicionando página {num}"));
                    var page = new Page { Number = num, Source = img.GetAttribute("src") };
                    pageList.Add(page);
                    num++;
                }
                return pageList;
            }
        }

        private IDocument GetPageListFromScript(IDocument source) {
            lock (Lock5) {
                var element = "";
                var imageElement = source.QuerySelector(".read-slideshow a");
                if (imageElement != null) {
                    var openTag = "<div class='pages-content'>";
                    var closeTag = "</div>";
                    var content = imageElement.ParentElement?.InnerHtml;
                    var html = source.Body.InnerHtml;
                    if (UsingIe) {
                        element = $"{openTag}{content}{closeTag}";
                    } else {
                        html = html.Substring(html.IndexOf("var images = ["));
                        html = html.Substring(html.IndexOf("["), html.IndexOf("];"));
                        var otherPages = html.Split(']')[0];
                        otherPages = otherPages.Split('[')[1];
                        otherPages = otherPages.Replace("\"", "");
                        element = $"{openTag}{content}{otherPages}{closeTag}";
                    }

                    element = element.Replace("\"", "'");
                    element = element.Replace(".webp", "");
                    element = element.Replace("/images", "/mangas_files");
                } else if (source.QuerySelector(".image-content a") != null) {
                    var openTag = "<div class='pages-content'>";
                    var closeTag = "</div>";
                    var images = "";
                    var html = source.Body.InnerHtml;
                    html = html.Substring(html.IndexOf("var pages = ["));
                    html = html.Substring(html.IndexOf("["), html.IndexOf("];"));
                    var otherPages = html.Split(']')[0];
                    otherPages = otherPages.Split('[')[1];
                    otherPages = otherPages.Replace("\"", "");
                    var linkPages = otherPages.Split(new string[] { "url:" }, StringSplitOptions.None);
                    foreach (var linkPage in linkPages) {
                        var el = linkPage.Split(',')[0];
                        if (!el.Contains("id")) {
                            var link = el.Replace("}", "");
                            link = link.Replace("'", "");
                            link = link.Replace("\\", "");
                            images += $"<img src='{link}' />";
                        }
                    }
                    element = $"{openTag}{images}{closeTag}";
                    element = element.Replace("\"", "'");
                    element = element.Replace(".webp", "");
                    element = element.Replace("/images", "/mangas_files");
                }

                return HtmlHelper.GetSourceCodeFromHtml(element);
            }
        }
    }
}
