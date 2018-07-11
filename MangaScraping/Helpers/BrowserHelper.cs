using DependencyInjectionResolver;
using MangaScraping.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MangaScraping.Helpers {
#pragma warning disable CS0618 // O tipo ou membro é obsoleto
    public class BrowserHelper {
        private PhantomJSDriverService _defaultService;
        private PhantomJSOptions _options;

        private readonly Object lockThis = new Object();
        private readonly Object lockThis2 = new Object();

        public BrowserHelper() {
            _defaultService = PhantomJSDriverService.CreateDefaultService(CoreConfiguration.WebDriversLocation);
            _defaultService.HideCommandPromptWindow = true;
            _options = new PhantomJSOptions();
            _options.AddAdditionalCapability("phantomjs.page.settings.userAgent",
                                            @"Mozilla/5.0 (Android 4.4; Mobile; rv:41.0) Gecko/41.0 Firefox/41.0");
        }

        public RemoteWebDriver GetDriver(string url) {
            lock (lockThis) {
                var driver = new PhantomJSDriver(_defaultService) {
                    Url = url
                };
                driver.Navigate();
                return driver;
            }
        }

        public RemoteWebDriver GetPhantomMobile(string url) {
            lock (lockThis2) {
                var driver = new PhantomJSDriver(_defaultService, _options) {
                    Url = url
                };
                driver.Navigate();
                return driver;
            }
        }

        public void CloseDriver() {
            Process[] pname = Process.GetProcessesByName("PhantomJS");
            try {
                foreach (Process proc in pname) {
                    proc.Kill();
                }
            } catch { }
        }
    }

#pragma warning restore CS0618 // O tipo ou membro é obsoleto
}
