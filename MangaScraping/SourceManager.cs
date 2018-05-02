using DependencyInjectionResolver;
using MangaScraping.Enumerators;
using MangaScraping.Helpers;
using MangaScraping.Managers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping {
    public class SourceManager {
        private SiteHelper _siteHelper;
        private MangaHostSourceManager _mangaHostManager;
        private UnionMangasSourceManager _unionManager;
        private YesMangasSourceManager _yesManager;
        private MangasProjectSourceManager _projectManager;
        private MangaLivreSourceManager _mangaLivreManager;
        private HipercoolSourceManager _hipercoolManager;
        private HqUltimateSourceManager _hqUltimateManager;
        private Dictionary<SourcesEnum, IHqSourceManager> _sources;

        private readonly Object _lockThis = new Object();

        public SourceManager(DependencyInjection dependencyInjection) {
            _mangaHostManager = dependencyInjection.Resolve<MangaHostSourceManager>();
            _unionManager = dependencyInjection.Resolve<UnionMangasSourceManager>();
            _yesManager = dependencyInjection.Resolve<YesMangasSourceManager>();
            _projectManager = dependencyInjection.Resolve<MangasProjectSourceManager>();
            _mangaLivreManager = dependencyInjection.Resolve<MangaLivreSourceManager>();
            //_hipercoolManager = dependencyInjection.Resolve<HipercoolSourceManager>();
           // _hqUltimateManager = dependencyInjection.Resolve<HqUltimateSourceManager>();
            _siteHelper = dependencyInjection.Resolve<SiteHelper>(); ;
            _sources = new Dictionary<SourcesEnum, IHqSourceManager> {
                [SourcesEnum.MangaHost] = _mangaHostManager,
                [SourcesEnum.UnionMangas] = _unionManager,
                [SourcesEnum.YesMangas] = _yesManager,
                [SourcesEnum.MangasProject] = _projectManager,
                [SourcesEnum.MangaLivre] = _mangaLivreManager,
                //[SourcesEnum.Hipercool] = _hipercoolManager,
                //[SourcesEnum.HqUltimate] = _hqUltimateManager
            };
        }

        public Dictionary<string, IHqSourceManager> GetSources() {
            lock (_lockThis) {

                var sourceNanagers = new Dictionary<string, IHqSourceManager> {
                    ["MangaHost"] = _mangaHostManager,
                    ["UnionMangas"] = _unionManager,
                    ["YesMangas"] = _yesManager,
                    ["MangasProject"] = _projectManager,
                    ["MangaLivre"] = _mangaLivreManager,
                    //["Hipercool"] = _hipercoolManager,
                   // ["HqUltimate"] = _hqUltimateManager
                };

                return sourceNanagers;
            }
        }

        public IHqSourceManager GetSourceFromLink(string link) => _siteHelper.GetHqSourceFromUrl(link);
        public IHqSourceManager GetSpurce(SourcesEnum source) => _sources[source];
    }
}
