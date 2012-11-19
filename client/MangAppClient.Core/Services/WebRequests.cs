namespace MangAppClient.Core.Services
{
    using MangAppClient.Core.Model;
    using MangAppClient.Core.Utilities;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Windows.Data.Json;

    public class WebRequests : IWebRequests
    {
        internal int MangaListVersion { get; private set; }

        // Working
        public void GetMangaChapters(Manga manga)
        {
            try
            {
                HttpClient client = new HttpClient();
                var response = client.GetStringAsync(string.Format(Urls.GetMangaDetail, manga.Key)).Result;

                // Transform JSON into manga
                this.ParseMangaChapters(manga, JObject.Parse(response));
            }
            catch (Exception)
            {
            }
        }

        // Working
        public void GetChapterPages(Chapter chapter)
        {
            try
            {
                HttpClient client = new HttpClient();
                var response = client.GetStringAsync(string.Format(Urls.GetMangaChapter, chapter.MangaKey, chapter.Key)).Result;

                // Transform JSON into chapter
                this.ParseChapterPages(chapter, JObject.Parse(response));
            }
            catch (HttpRequestException)
            {
            }
        }

        public void GetChapterPages(Chapter chapter, int providerKey)
        {
            try
            {
                HttpClient client = new HttpClient();
                var response = client.GetStringAsync(string.Format(Urls.GetMangaChapterFromProvider, chapter.MangaKey, chapter.Key, providerKey)).Result;

                // Transform JSON into manga
                this.ParseChapterPages(chapter, JObject.Parse(response));
            }
            catch (HttpRequestException)
            {
            }
        }

        public IEnumerable<Manga> GetRelatedMangas(Manga manga)
        {
            try
            {
                HttpClient client = new HttpClient();
                var response = client.GetStringAsync(string.Format(Urls.GetRelatedMangas, manga.Key)).Result;

                // Transform JSON into objects
                List<Manga> results = new List<Manga>();
                results.AddRange(JObject.Parse(response).Children().Select(t => this.ParseManga(t)));

                return results;
            }
            catch (HttpRequestException)
            {
                return Enumerable.Empty<Manga>();
            }
        }

        public IEnumerable<int> GetFavoriteMangas(Guid userId)
        {
            try
            {
                HttpClient client = new HttpClient();
                var response = client.GetStringAsync(string.Format(Urls.GetFavoriteMangas, userId.ToString())).Result;

                // Transform JSON into objects
                List<int> results = new List<int>();
                results.AddRange(JObject.Parse(response).Children().Values<int>());

                return results;
            }
            catch (HttpRequestException)
            {
                return Enumerable.Empty<int>();
            }
        }

        public void DownloadMangaChapter(Chapter chapter)
        {
            throw new NotImplementedException();
        }

        public void DownloadMangaChapters(Chapter chapterStart, Chapter chapterEnd)
        {
            throw new NotImplementedException();
        }

        // Working
        internal IEnumerable<Manga> GetMangaList()
        {
            try
            {
                HttpClient client = new HttpClient();
                var response = client.GetStringAsync(Urls.GetMangaList).Result;

                // Transform JSON into objects
                JObject json = JObject.Parse(response);

                this.MangaListVersion = json["version"].Value<int>();

                List<Manga> results = new List<Manga>();
                results.AddRange(json["mangas"].Children().Select(t => this.ParseManga(t)));

                return results.OrderByDescending(m => m.Popularity);
            }
            catch (HttpRequestException)
            {
                return Enumerable.Empty<Manga>();
            }
        }

        internal IEnumerable<DiffResult> GetMangaListDiff(int localListVersion)
        {
            try
            {
                List<DiffResult> results = new List<DiffResult>();

                //HttpClient client = new HttpClient();
                //var response = client.GetStringAsync(string.Format(Urls.GetMangaDiff, localListVersion)).Result;

                //// Transform JSON into object
                //JObject json = JObject.Parse(response);

                //this.MangaListVersion = json["version"].Value<int>();
                //var groups = json["manga"].Children().GroupBy(t => t["operation"].Value<string>());

                //// Get the mangas that were deleted
                //results.AddRange(groups
                //    .Where(group => group.Key.Equals("delete", StringComparison.CurrentCultureIgnoreCase))
                //    .SelectMany(group => group)
                //    .Select(item => new RemoveDiffResult(item["id"].Value<string>())));

                //// Get the mangas that were updated
                //results.AddRange(groups
                //    .Where(group => group.Key.Equals("update", StringComparison.CurrentCultureIgnoreCase))
                //    .SelectMany(group => group)
                //    .Select(item => new UpdateDiffResult(
                //        item["id"].Value<string>(),
                //        item["chapter"].Value<int>(),
                //        string.IsNullOrEmpty(item["status"].Value<string>()) ? null : (MangaStatus?)Enum.Parse(typeof(MangaStatus), item["status"].Value<string>()))));

                //// Get the mangas that were added
                //results.AddRange(groups
                //    .Where(group => group.Key.Equals("add", StringComparison.CurrentCultureIgnoreCase))
                //    .SelectMany(group => group)
                //    .Select(item => this.ParseManga(item)));

                return results;
            }
            catch (HttpRequestException)
            {
                return Enumerable.Empty<DiffResult>();
            }
        }

        internal byte[] GetBackgroundImage(string mangaId)
        {
            try
            {
                return null;

                HttpClient client = new HttpClient();
                return client.GetByteArrayAsync(string.Format(Urls.GetBackgroundImage, mangaId)).Result;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        // Working
        private Manga ParseManga(JToken token)
        {
            Manga manga = new Manga();

            manga.Key = token["_id"].Value<string>();
            manga.Title = token["title"].Value<string>();
            manga.Description = token["description"].Value<string>();
            manga.AlternativeNamesDb = string.Join("#", token["alias"].Children().Values<string>());
            manga.Popularity = JsonHelper.ParseInt(token["hits"]);

            manga.ProvidersDb = string.Join("#", token["providers"].Children().Values<string>());

            manga.AuthorsDb = string.Join("#", token["authors"].Children().Values<string>());
            manga.ArtistsDb = string.Join("#", token["artists"].Children().Values<string>());
            manga.CategoriesDb = string.Join("#", token["categories"].Children().Values<string>());

            manga.YearOfRelease = this.ParseYear(JsonHelper.ParseInt(token["released"]));
            manga.StatusDb = JsonHelper.ParseInt(token["status"]);
            manga.ReadingDirectionDb = JsonHelper.ParseInt(token["direction"]);

            manga.RemoteSummaryImageDb = token["image"].Value<string>();
            manga.LocalSummaryImage = null;

            manga.LastChapter = JsonHelper.ParseInt(token["chapters_len"]);
            manga.LastChapterDate = this.ParseDateTime(JsonHelper.ParseInt(token["last_chapter_date"]));
            manga.LastChapterRead = null;

            JToken chapters = token["chapters"];
            if (chapters != null)
            {
                manga.Chapters = chapters.Children().Select(c => this.ParseChapter(manga.Key, c)).OrderBy(c => c.Number);
            }
            return manga;
        }

        private void ParseMangaChapters(Manga manga, JToken token)
        {
            JToken chapters = token["chapters"];
            if (chapters != null)
            {
                manga.Chapters = chapters.Children().Select(c => this.ParseChapter(manga.Key, c)).OrderBy(c => c.Number);
            }
        }

        // Working
        private Chapter ParseChapter(string mangaKey, JToken chapterJson)
        {
            Chapter chapter = new Chapter();

            chapter.Key = chapterJson["_id"].Value<string>();
            chapter.MangaKey = mangaKey;
            chapter.ProviderKey = JsonHelper.ParseString(chapterJson["provider"]);
            chapter.PreviousChapterId = JsonHelper.ParseString(chapterJson["previous"]);
            chapter.NextChapterId = JsonHelper.ParseString(chapterJson["next"]);
            chapter.Number = JsonHelper.ParseInt(chapterJson["number"]);
            chapter.Title = JsonHelper.ParseString(chapterJson["title"]);
            chapter.Pages = this.ParsePages(chapterJson["pages"].Children());

            return chapter;
        }

        private void ParseChapterPages(Chapter chapter, JToken chapterJson)
        {
            chapter.Pages = this.ParsePages(chapterJson["pages"].Children());
        }

        // Working
        private List<string> ParsePages(IEnumerable<JToken> pages)
        {
            return pages
                .Select(t => new { Number = t["number"].Value<int>(), Url = t["url"].Value<string>() })
                .OrderBy(p => p.Number)
                .Select(p => p.Url).ToList();
        }

        // Working
        private DateTime? ParseDateTime(int? days)
        {
            if (days.HasValue)
            {
                DateTime dateTime = new DateTime();
                return dateTime.AddDays(days.Value);
            }

            return null;
        }

        // Working
        private int? ParseYear(int? days)
        {
            if (days.HasValue)
            {
                DateTime dateTime = new DateTime();
                return dateTime.AddDays(days.Value).Year;
            }

            return null;
        }
    }
}
