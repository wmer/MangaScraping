using MangaScraping.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Managers {
    public interface IHqSourceManager {
        IHqSourceManager Search(string hqTitle, out List<Hq> result);
        IHqSourceManager GetInfo<U>(string url, out U model, double timeCache = 3000, bool isFinalized = false, bool whithoutCache = false) where U : ModelBase;
        IHqSourceManager GetUpdates(out List<Update> updates, double timeCache = 189);
        IHqSourceManager GetLibrary(out List<Hq> library, double timeCache = 4320);
        IHqSourceManager GetFinalizedPage(out List<Hq> library, double timeCache = 4320);
        IHqSourceManager GetLetherPage(string lether, out List<Hq> library, double timeCache = 4320);
        IHqSourceManager NextLibraryPage(out List<Hq> library, double timeCache = 4320);
    }
}
