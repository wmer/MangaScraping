using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MangaScraping.Helpers {
    internal static class InternetChecker {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

        public static bool IsConnectedToInternet() {
            return InternetGetConnectedState(out int Desc, 0);
        }
    }
}
