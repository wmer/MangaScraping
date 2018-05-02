using MangaScraping.Models.MangasProjectApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace MangaScraping.Helpers {
    public class ProjectApiConsumer {
        private HttpClient _client;

        public ProjectApiConsumer() {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
        }

        public List<Release> GetReleases(string baseUrl) {
            var response = _client.GetAsync($"{baseUrl}/home/releases.json").Result;
            if (response.IsSuccessStatusCode) {
                response.EnsureSuccessStatusCode();
                string responseBody = response.Content.ReadAsStringAsync().Result;
                var rootObject = JsonConvert.DeserializeObject<RootObject>(responseBody);
                return rootObject.releases;
            }else {
                return new List<Release>();
            }
        }

        public List<Chapter> GetChapters(string baseUrl, string serieId) {
            var page = 1;
            var hasChapter = true;
            var chapters = new List<Chapter>();
            while (hasChapter) {
                var response = _client.GetAsync($"{baseUrl}/series/chapters_list.json?page={page}&id_serie={serieId}").Result;
                if (response.IsSuccessStatusCode) {
                    response.EnsureSuccessStatusCode();
                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    try {
                        var rootObject = JsonConvert.DeserializeObject<RootObject>(responseBody);
                        chapters.AddRange(rootObject.chapters);
                        page++;
                    } catch {
                        hasChapter = false;
                    }
                }
            }

            return chapters;
        }

        public List<string> GetPages(string baseUrl, string releaseId, string token) {
            var response = _client.GetAsync($"{baseUrl}/leitor/pages.json?key={token}& id_release={releaseId}").Result;
            if (response.IsSuccessStatusCode) {
                response.EnsureSuccessStatusCode();
                string responseBody = response.Content.ReadAsStringAsync().Result;
                var rootObject = JsonConvert.DeserializeObject<RootObject>(responseBody);
                return rootObject.images;
            } else {
                return new List<string>();
            }
        }

        public List<Series> Search(string baseUrl, string term) {
            var dic = new Dictionary<String, string>() {
                {"search", term }
            };
            var httpContent = new FormUrlEncodedContent(dic);
            var response = _client.PostAsync($"{baseUrl}/lib/search/series.json", httpContent).Result;
            string responseBody = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode) {
                response.EnsureSuccessStatusCode();
                var rootObject = JsonConvert.DeserializeObject<RootObject>(responseBody);
                return rootObject.series;
            } else {
                return new List<Series>();
            }
        }
    }
}
