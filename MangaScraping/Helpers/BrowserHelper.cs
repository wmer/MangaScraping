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
    public class BrowserHelper {

        private Object lockThis = new Object();
        private Object lockThis2 = new Object();
        private Object lockThis3 = new Object();

        public RemoteWebDriver GetDriver(string url) {
            lock (lockThis) {
                CheckAndCloseIfRunning();
                var driverService = PhantomJSDriverService.CreateDefaultService(CoreConfiguration.WebDriversLocation);
                driverService.HideCommandPromptWindow = true;
#pragma warning disable CS0618 // O tipo ou membro é obsoleto
                var driver = new PhantomJSDriver(driverService) {
#pragma warning restore CS0618 // O tipo ou membro é obsoleto
                    Url = url
                };
                driver.Navigate();
                return driver;
            }
        }

        public RemoteWebDriver GetPhantomMobile(string url) {
            lock (lockThis2) {
                CheckAndCloseIfRunning();
                var driverService = PhantomJSDriverService.CreateDefaultService(CoreConfiguration.WebDriversLocation);
                driverService.HideCommandPromptWindow = true;
                var options = new PhantomJSOptions();
                options.AddAdditionalCapability("phantomjs.page.settings.userAgent",
               @"Mozilla/5.0 (Android 4.4; Mobile; rv:41.0) Gecko/41.0 Firefox/41.0");
               
#pragma warning disable CS0618 // O tipo ou membro é obsoleto
                var driver = new PhantomJSDriver(driverService, options) {
#pragma warning restore CS0618 // O tipo ou membro é obsoleto
                    Url = url
                };
                
                driver.Navigate();
                return driver;
            }
        }

        public void CheckAndCloseIfRunning() {
            lock (lockThis3) {
                Process[] pname = Process.GetProcessesByName("PhantomJS");
                try {
                    foreach (Process proc in pname) {
                        proc.Kill();
                    }
                } catch { }
            }
        }
    }
}
