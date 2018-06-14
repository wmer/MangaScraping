using AngleSharp.Dom;
using MangaScraping.Databases;
using MangaScraping.Events;
using MangaScraping.Helpers;
using MangaScraping.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp;

namespace MangaScraping.Sources {
    public class MangasProjectSource : HqSource {
        private ProjectApiConsumer _apiConsumer;
        private string BaseAdress { get; set; }

        public MangasProjectSource(LibraryContext libraryContext, HtmlSourceHelper htmlHelper, BrowserHelper browserHelper, ProjectApiConsumer apiConsumer) : base(libraryContext, htmlHelper, browserHelper) {
            _apiConsumer = apiConsumer;
        }

        public List<Hq> Search(string link, string hqTitle) {
            lock (Lock8) {
                var result = new List<Hq>();
                var searchResult = _apiConsumer.Search(link, hqTitle);

                OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"{searchResult.Count()} mangas encontrados!"));
                if (searchResult.Count() > 0) {
                    foreach (var hqEl in searchResult) {
                        result.Add(new Hq {
                            Link = $"{link}{hqEl.link}",
                            Title = CleanTitle(hqEl.name),
                            CoverSource = hqEl.cover,
                            Author = hqEl.author,
                            IsFinalized = hqEl.is_complete
                        });
                    }
                } else {
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Não foi encontrado resultados!"));
                }

                OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Tudo pronto"));
                return result;
            }
        }

        public override List<Update> GetUpdates(string url) {
            lock (Lock1) {
                try {
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Buscando Atualizações..."));
                    Uri site = new Uri(url);
                    BaseAdress = $"{site.Scheme}://{site.Host}";
                    var releases = _apiConsumer.GetReleases(BaseAdress);
                    var updates = new List<Update>();
                    if (releases == null) throw new Exception("Ocorreu um erro ao buscar informaçoes da Hq");
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"{releases.Count()} mangas encontrados!"));
                    foreach (var update in releases) {
                        var title = CleanTitle(update.name);
                        var img = update.image;
                        var link = update.link;

                        var chapters = new List<Chapter>();
                        foreach (var chap in update.chapters) {
                            var chapterTitle = $"Capitulo {chap.number}";
                            var chapterLink = chap.url;
                            chapters.Add(new Chapter { Link = $"{BaseAdress}{chapterLink}", Title = chapterTitle });
                        }
                        var up = new Update {
                            Hq = new Hq { Link = $"{BaseAdress}{link}", Title = title, CoverSource = img },
                            Chapters = chapters
                        };
                        updates.Add(up);
                        OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, up.Hq, $"{title} Adicionado"));
                    }
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Tudo pronto"));
                    return updates;
                } catch (Exception e) {
                    OnProcessingProgressError(new ProcessingErrorEventArgs(DateTime.Now, url, e));
                    return new List<Update>();
                }
            }
        }

        public override LibraryPage GetLibrary(String linkPage) {
            lock (Lock2) {
                try {
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, "Processando os Mangas..."));
                    Uri site = new Uri(linkPage);
                    BaseAdress = $"{site.Scheme}://{site.Host}";
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Abrindo Internet Explorer"));
                    var driver = BrowserHelper.GetDriver(linkPage);
                    var pageSource = driver.PageSource;
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Pegando dados da página"));
                    IDocument source = HtmlHelper.GetSourceCodeFromHtml(pageSource);
                    driver.Quit();
                    var hqs = new List<Hq>();
                    if (source == null) throw new Exception("Ocorreu um erro ao buscar informaçoes da Hq");
                    var hqsEl = source.QuerySelectorAll(".content-wraper ul.seriesList li");
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"{hqsEl.Count()} mangas encontrados!"));
                    var nextPage = source.QuerySelector("ul.content-pagination li.next a")?.GetAttribute("href");
                    var finalizadosEl = source.QuerySelectorAll("#subtopnav ul#menu-titulos li a");
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Pegando links"));
                    var finalizedPage = "";
                    foreach (var el in finalizadosEl) {
                        var test = el.TextContent;
                        if (el.TextContent.Contains("Recém Finalizados")) {
                            finalizedPage = el.GetAttribute("href");
                        }
                    }
                    var lethers = source.QuerySelectorAll("#az-order div a");
                    var letherLink = new Dictionary<string, string>();
                    foreach (var lether in lethers) {
                        letherLink[lether.TextContent.Trim()] = $"{BaseAdress}{lether.GetAttribute("href")}";
                    }
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Preparando para retornar mangas!"));
                    foreach (var hq in hqsEl) {
                        try {
                            var title = hq.QuerySelector(".series-title h1")?.TextContent;
                            title = CleanTitle(title);
                            var imgEl = hq.QuerySelector(".cover-image")?.GetAttribute("style");
                            imgEl = imgEl?.Replace("background-image: url(", "");
                            imgEl = imgEl?.Replace(")", "");
                            imgEl = imgEl?.Replace("'", "");
                            imgEl = imgEl?.Replace("\"", "");
                            var img = imgEl?.Replace(";", "");
                            var link = hq.QuerySelector("a")?.GetAttribute("href");
                            if (title != null) {
                                OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Pegando dados de {title}"));
                                if (!img.Contains(site.Scheme)) {
                                    img = $"{BaseAdress}{img}";
                                }
                                if (!string.IsNullOrEmpty(link)) {
                                    var manga = new Hq { Link = $"{BaseAdress}{link}", Title = title, CoverSource = img };
                                    if (!hqs.Contains(manga)) {
                                        hqs.Add(manga);
                                        OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, manga, $"{title} Adicionado"));
                                    }
                                }
                            }
                        } catch {}                     
                    }
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Tudo pronto"));
                    return new LibraryPage {
                        Link = linkPage, Hqs = hqs, NextPage = $"{BaseAdress}{nextPage}",
                        FinalizedPage = $"{BaseAdress}{finalizedPage}", Letras = letherLink
                    };
                } catch (Exception e) {
                    OnProcessingProgressError(new ProcessingErrorEventArgs(DateTime.Now, linkPage, e));
                    return null;
                }
            }
        }

        public override Hq GetHqInfo(string link) {
            lock (Lock3) {
                try {
                    Uri site = new Uri(link);
                    BaseAdress = $"{site.Scheme}://{site.Host}";

                    var driver = BrowserHelper.GetDriver(link);
                    var pageSource = driver.PageSource;
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Pegando dados da página"));
                    IDocument source = HtmlHelper.GetSourceCodeFromHtml(pageSource);
                    driver.Quit();

                    var hqInfo = new Hq();

                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Pegando dados da página"));
                    if (source == null) throw new Exception("Ocorreu um erro ao buscar informaçoes da Hq");
                    var title = source.QuerySelector("div#series-data div.series-info span.series-title h1")?.TextContent;
                    title = CleanTitle(title);
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Buscando informações de {title}"));
                    var img = source.QuerySelector("div.series-img div.cover img");
                    var author = source.QuerySelector("div#series-data div.series-info span.series-author")?.TextContent;
                    var synopsis = source.QuerySelector("div#series-data div.series-info span.series-desc")?.TextContent;
                    hqInfo.Title = title;
                    hqInfo.Author = author.Replace("\n", "").Trim().Replace("\n", "").Trim().Replace("'", "`").Trim();
                    hqInfo.CoverSource = img?.GetAttribute("src");
                    hqInfo.Synopsis = synopsis.Replace("\n", "").Trim().Replace("\n", "").Trim().Replace("'", "`").Trim();
                    hqInfo.Link = link;
                    var lastchapter = source.QuerySelector("#chapter-list .list-of-chapters li a");
                    var chapters = GetListChapters(link).Reverse<Chapter>().ToList();
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
            lock (Lock4) {
                try {
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Abrindo Internet Explorer"));
                    var driver = BrowserHelper.GetDriver(link);
                    var pageSource = driver.PageSource;
                    Task.Delay(5000);
                    var token = pageSource.Substring(pageSource.IndexOf("reader.min.js"));
                    token = token.Substring(token.IndexOf("token"));
                    token = token.Substring(0, token.IndexOf("id_release"));
                    token = token.Substring(0, token.Length - 5);
                    token = token.Replace("token=", "");
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Pegando dados da página"));
                    IDocument leitor = HtmlHelper.GetSourceCodeFromHtml(pageSource);
                    var tes = leitor.QuerySelectorAll(".requests");
                    var chapter = new Chapter();

                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Pegando dados da página"));
                    if (leitor == null) {
                        driver.Quit();
                        throw new Exception("Ocorreu um erro ao buscar informaçoes do capitulo");
                    }

                    var title = leitor.QuerySelector(".series-info .series-title .title")?.TextContent;
                    var chap = leitor.QuerySelector(".chapter-selection .current-chapter")?.TextContent;
                    var chapterTitle = $"{title} - {chap}";
                    chapterTitle = CleanTitle(chapterTitle);
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Buscando informações de {chapterTitle}"));
                    chapter.Title = CleanTitle(chapterTitle);
                    chapter.Link = link;
                    List<Page> pageList = GetPageList(link, token);
                    if (pageList.Count() > 1) {
                        chapter.Pages = pageList;
                    } else {
                        driver.Quit();
                        throw new Exception("Ocorreu um erro ao buscar informaçoes do capitulo");
                    }
                    driver.Quit();
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Tudo pronto"));
                    return chapter;
                } catch (Exception e) {
                    OnProcessingProgressError(new ProcessingErrorEventArgs(DateTime.Now, link, e));
                    return null;
                }
            }
        }

        public List<Chapter> GetListChapters(String link) {
            lock (Lock5) {
                Uri site = new Uri(link);
                OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Buscando capitulos"));
                var chapters = _apiConsumer.GetChapters(BaseAdress, site.Segments.Last());
                var chapterList = new List<Chapter>();
                if (chapters != null) {
                    foreach (var chapter in chapters) {
                        try {
                            var linkChap = chapter.releases.Values.FirstOrDefault().link;
                            var chapterLink = $"{BaseAdress}{linkChap}";
                            var chapterTitle = $"Capitulo - {chapter.number} - {chapter.chapter_name}";
                            chapterTitle = CleanTitle(chapterTitle);
                            chapterList.Add(new Chapter { Title = chapterTitle, Link = chapterLink });
                        }catch { }
                    }
                }

                return chapterList;
            }
        }

        public List<Page> GetPageList(string link, string token) {
            lock (Lock6) {
                Uri site = new Uri(link);
                BaseAdress = $"{site.Scheme}://{site.Host}";
                OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Buscando páginas"));
                var pageList = new List<Page>();
                var pages = _apiConsumer.GetPages(BaseAdress, (site.Segments[site.Segments.Count() - 2]).Replace("/", ""), token);

                var num = 1;
                foreach (var img in pages) {
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Adicionando página {num}"));
                    var page = new Page { Number = num, Source = img };
                    pageList.Add(page);
                    num++;
                }


                return pageList;
            }
        }
    }
}
