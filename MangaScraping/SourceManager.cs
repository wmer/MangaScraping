using DependencyInjectionResolver;
using MangaScraping.Enumerators;
using MangaScraping.Helpers;
using MangaScraping.Managers;
using MangaScraping.Sources;
using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping {
    public class SourceManager {
        private SiteHelper _siteHelper;
        private MangaHostSourceManager _mangaHostManager;
        private HqSourceManager<UnionMangasSource> _unionManager;
        private HqSourceManager<YesMangasSource> _yesManager;
        private MangasProjectSourceManager _projectManager;
        private MangaLivreSourceManager _mangaLivreManager;
        private Dictionary<SourcesEnum, IHqSourceManager> _sources;
        private Dictionary<string, IHqSourceManager> _sourceNanagers;
        

        public SourceManager(DependencyInjection dependencyInjection) {
            _mangaHostManager = dependencyInjection
                                    .DefineDependency<MangaHostSourceManager>(0, "https://mangahost-br.com/")
                                    .DefineDependency<MangaHostSourceManager>(1, "https://mangahost-br.com/mangas")
                                    .Resolve<MangaHostSourceManager>();

            _unionManager = dependencyInjection
                                    .DefineDependency<HqSourceManager<UnionMangasSource>>(0, "http://unionmangas.cc/")
                                    .DefineDependency<HqSourceManager<UnionMangasSource>>(1, "http://unionmangas.cc/mangas")
                                    .Resolve<HqSourceManager<UnionMangasSource>>();

            _yesManager = dependencyInjection
                                    .DefineDependency<HqSourceManager<YesMangasSource>>(0, "https://yesmangasbr.com/")
                                    .DefineDependency<HqSourceManager<YesMangasSource>>(1, "https://yesmangasbr.com/mangas")
                                    .Resolve<HqSourceManager<YesMangasSource>>();

            _projectManager = dependencyInjection
                                    .DefineDependency<MangasProjectSourceManager>(0, "https://leitor.net")
                                    .DefineDependency<MangasProjectSourceManager>(1, "https://leitor.net/lista-de-mangas/ordenar-por-nome/todos")
                                    .Resolve<MangasProjectSourceManager>();

            _mangaLivreManager = dependencyInjection
                                    .DefineDependency<MangaLivreSourceManager>(0, "https://mangalivre.com/")
                                    .DefineDependency<MangaLivreSourceManager>(1, "https://mangalivre.com/lista-de-mangas/ordenar-por-nome/todos")
                                    .Resolve<MangaLivreSourceManager>();


            _siteHelper = dependencyInjection.Resolve<SiteHelper>();
            _sources = new Dictionary<SourcesEnum, IHqSourceManager> {
                [SourcesEnum.MangaHost] = _mangaHostManager,
                [SourcesEnum.UnionMangas] = _unionManager,
                [SourcesEnum.YesMangas] = _yesManager,
                [SourcesEnum.MangasProject] = _projectManager,
                [SourcesEnum.MangaLivre] = _mangaLivreManager,
            };

            _sourceNanagers = new Dictionary<string, IHqSourceManager> {
                ["MangaHost"] = _mangaHostManager,
                ["UnionMangas"] = _unionManager,
                ["YesMangas"] = _yesManager,
                ["MangasProject"] = _projectManager,
                ["MangaLivre"] = _mangaLivreManager,
            };
        }

        public Dictionary<string, IHqSourceManager> GetSources() => _sourceNanagers;
        public IHqSourceManager GetSourceFromLink(string link) => _siteHelper.GetHqSourceFromUrl(link);
        public IHqSourceManager GetSpurce(SourcesEnum source) => _sources[source];
    }
}
