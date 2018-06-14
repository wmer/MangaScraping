using AngleSharp.Dom;
using MangaScraping.Databases;
using MangaScraping.Events;
using MangaScraping.Helpers;
using MangaScraping.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MangaScraping.Sources {
    public class UnionMangasSource : HqSource {

        public UnionMangasSource(LibraryContext libraryContext, HtmlSourceHelper htmlHelper, BrowserHelper browserHelper) : base(libraryContext, htmlHelper, browserHelper) {
        }

        public override List<Update> GetUpdates(string url) {
            lock (Lock6) {
                try {
                    var hqs = new List<Update>();
                    throw new Exception("Ocorreu um erro ao buscar informaçoes da Hq");
                } catch (Exception) {
                    return new List<Update>();
                }

            }
        }

        public override LibraryPage GetLibrary(string linkPage) {
            lock (Lock5) {
                try {
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, "Processando os Mangas..."));
                    var source = HtmlHelper.GetSourceCodeFromUrl(linkPage);
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Pegando dados da página"));
                    var hqs = new List<Hq>();
                    if (source == null) throw new Exception("Ocorreu um erro ao buscar informaçoes da Hq");
                    var hqsEl = source.QuerySelectorAll("div.bloco-manga");
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"{hqsEl.Count()} mangas encontrados!"));
                    var nextPageAll = source.QuerySelectorAll("ul.pagination li a");
                    var nextPage = "";
                    foreach (var el in nextPageAll) {
                        if (el.QuerySelector("span") != null) {
                            if (el.QuerySelector("span").TextContent.Contains("›")) {
                                nextPage = el.GetAttribute("href");
                            }
                        }
                    }
                    var lethers = source.QuerySelectorAll("nav.lista-manga ul li");
                    var letherLink = new Dictionary<string, string>();
                    foreach (var lether in lethers) {
                        letherLink[lether.QuerySelector("a")?.TextContent] = lether.QuerySelector("a")?.GetAttribute("href");
                    }
                    var finalizedPage = "";
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Preparando para retornar mangas!"));
                    foreach (var hq in hqsEl) {
                        try {
                            var title = hq.QuerySelector("div b")?.TextContent;
                            title = CleanTitle(title);
                            var img = hq.QuerySelector("a img")?.GetAttribute("src");
                            var link = hq.QuerySelector("a")?.GetAttribute("href");
                            if (!string.IsNullOrEmpty(link)) {
                                var manga = new Hq { Link = link, Title = title, CoverSource = img };
                                hqs.Add(manga);
                                OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, manga, $"{title} Adicionado"));
                            }
                        } catch { }
                    }
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Tudo pronto"));
                    return new LibraryPage {
                        Link = linkPage, Hqs = hqs, NextPage = nextPage,
                        FinalizedPage = finalizedPage, Letras = letherLink
                    };
                } catch (Exception e) {
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
                    var title = source.QuerySelector(".tamanho-bloco-perfil h2")?.TextContent;
                    title = CleanTitle(title);
                    var img = source.QuerySelector("img.img-thumbnail");
                    var synopsis = source.QuerySelector(".tamanho-bloco-perfil div.panel-body")?.TextContent;
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Buscando informações de {title}"));
                    hqInfo.Title = title;
                    hqInfo.CoverSource = img?.GetAttribute("src");
                    hqInfo.Link = link;
                    hqInfo.Synopsis = synopsis?.Replace("\n", "").Trim().Replace("'", "`").Trim();
                    var hqInfos = source.QuerySelectorAll(".tamanho-bloco-perfil h4.manga-perfil");
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
                    var leitor = HtmlHelper.GetSourceCodeFromUrl(link);
                    var chapter = new Chapter();
                    if (leitor == null) throw new Exception("Ocorreu um erro ao buscar informaçoes do capitulo");
                    var chapterTitle = leitor.QuerySelector(".container .titulo-leitura")?.TextContent;
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Buscando informações de {chapterTitle}"));
                    chapter.Title = CleanTitle(chapterTitle);
                    chapter.Link = link;
                    var pages = GetPageList(leitor);
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
                var chapters = hqSource.QuerySelectorAll(".lancamento-linha div:nth-child(1) a");
                if (chapters != null) {
                    foreach (var chapter in chapters) {
                        var chapterLink = chapter.GetAttribute("href");
                        var chapterTitle = $"Capitulo - {chapter.TextContent.Replace("Cap. ", "")}";
                        chapterTitle = CleanTitle(chapterTitle);
                        OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Adicionando {chapterTitle}"));
                        chapterList.Add(new Chapter { Link = chapterLink, Title = chapterTitle });
                    }
                }
                return chapterList;
            }
        }

        public List<Page> GetPageList(IDocument chapterSoure) {
            lock (Lock4) {
                OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Buscando páginas"));
                var pageList = new List<Page>();
                var pages = chapterSoure.QuerySelectorAll(".img-manga");
                if (pages != null) {
                    var num = 1;
                    foreach (var page in pages) {
                        var imgLink = $"http:{page.GetAttribute("src")}";
                        OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Adicionando página {num}"));
                        pageList.Add(new Page { Number = num, Source = imgLink });
                        num++;
                    }
                }
                return pageList;
            }
        }
    }
}
