using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Configuration {
    public class CoreConfiguration {
        public static string BaseDirectory { get; set; } = AppDomain.CurrentDomain.BaseDirectory;

        private static string _cacheLocation = $"{BaseDirectory}\\Cache";
        private static string _coverLocation = $"{CacheLocation}\\Covers";

        public static string CacheLocation {
            get { return _cacheLocation; }
            set { _cacheLocation = value; }
        }

        public static string WebDriversLocation { get; set; } = $"{BaseDirectory}\\WebDrivers";
        public static string DatabaseLocation { get; set; } = $"{BaseDirectory}\\Databases";

        public static string CoverLocaion {
            get => _coverLocation;
            set { _coverLocation = value; }
        }

    }
}
