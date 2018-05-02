using MangaScraping.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraping.Helpers {
    public class BrowserHelper {

        private Object lockThis = new Object();
        private Object lockThis2 = new Object();

        public RemoteWebDriver GetDriver(string url) {
            lock (lockThis) {
#pragma warning disable CS0618 // O tipo ou membro é obsoleto
                var driver = new PhantomJSDriver(CoreConfiguration.WebDriversLocation) {
#pragma warning restore CS0618 // O tipo ou membro é obsoleto
                    Url = url
                };
                driver.Navigate();
                return driver;
            }
        }

        public RemoteWebDriver GetPhantomMobile(string url) {
            lock (lockThis2) {
                var options = new PhantomJSOptions();
                options.AddAdditionalCapability("phantomjs.page.settings.userAgent",
               @"Mozilla/5.0 (Android 4.4; Mobile; rv:41.0) Gecko/41.0 Firefox/41.0");

#pragma warning disable CS0618 // O tipo ou membro é obsoleto
                var driver = new PhantomJSDriver(CoreConfiguration.WebDriversLocation, options) {
#pragma warning restore CS0618 // O tipo ou membro é obsoleto
                    Url = url
                };
                //IJavaScriptExecutor js = driver;
                //var requests = js.ExecuteScript(
                //    "var currentRequests = {};" +
                //    "$.ajaxSetup({" +
                //        "beforeSend: function (xhr,settings) {" +
                //        "$(body).append('<p class=\"requests\">settings.url</p>');" +
                //      "}" +
                //    "});" +
                //    "while(document.readyState != 'complete'){}" +
                //    "return currentRequests;");

                driver.Navigate();


                return driver;
            }
        }
    }
}
