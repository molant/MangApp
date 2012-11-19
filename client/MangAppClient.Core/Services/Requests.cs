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

    public class Requests : IRequests
    {
        internal int MangaListVersion { get; private set; }

        // Working
        public Manga GetMangaDetail(Manga manga)
        {
            try
            {
                HttpClient client = new HttpClient();
                var response = client.GetStringAsync(string.Format(Urls.GetMangaDetail, manga.Key)).Result;

                // Transform JSON into manga
                return this.ParseManga(JObject.Parse(response));
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Working
        public Chapter GetChapter(Manga manga, Chapter chapter)
        {
            try
            {
                HttpClient client = new HttpClient();
                var response = client.GetStringAsync(string.Format(Urls.GetMangaChapter, manga.Key, chapter.Key)).Result;

                // Transform JSON into chapter
                return this.ParseChapter(manga.Key, JObject.Parse(response));
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public Chapter GetChapterFromProvider(Manga manga, Chapter chapter, int providerKey)
        {
            try
            {
                HttpClient client = new HttpClient();
                var response = client.GetStringAsync(string.Format(Urls.GetMangaChapterFromProvider, manga.Key, chapter.Key, providerKey)).Result;

                // Transform JSON into manga
                JObject json = JObject.Parse(response);
                return this.ParseChapter(manga.Key, json["chapter"]);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public IEnumerable<Manga> GetRelatedMangas(Manga manga)
        {
            try
            {
                List<Manga> results = new List<Manga>();

                HttpClient client = new HttpClient();
                var response = client.GetStringAsync(string.Format(Urls.GetRelatedMangas, manga.Key)).Result;

                // Transform JSON into objects
                results.AddRange(JObject.Parse(response).Children().Select(t => this.ParseManga(t)));

                return results;
            }
            catch (HttpRequestException)
            {
                return Enumerable.Empty<Manga>();
            }
        }

        public IEnumerable<int> GetFavoriteMangas(int userId)
        {
            try
            {
                List<int> results = new List<int>();

                HttpClient client = new HttpClient();
                var response = client.GetStringAsync(string.Format(Urls.GetFavoriteMangas, userId)).Result;

                // Transform JSON into objects
                JObject json = JObject.Parse(response);
                results.AddRange(json["favorite"].Children().Values<int>());

                return results;
            }
            catch (HttpRequestException)
            {
                return Enumerable.Empty<int>();
            }
        }

        // Working
        internal IEnumerable<Manga> GetMangaList()
        {
            try
            {
                List<Manga> results = new List<Manga>();

                HttpClient client = new HttpClient();
                var response = client.GetStringAsync(Urls.GetMangaList).Result;

                // Transform JSON into objects
                JArray json = JArray.Parse(response);

                this.MangaListVersion = 1; // json["version"].Value<int>();
                results.AddRange(json.Children().Select(t => this.ParseManga(t)));

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
            manga.Status = this.ParseMangaStatus(JsonHelper.ParseInt(token["status"]));
            manga.ReadingDirection = this.ParseReadingDirection(JsonHelper.ParseInt(token["direction"]));

            manga.RemoteSummaryImageDb = token["image"].Value<string>();
            manga.LocalSummaryImage = null;

            manga.LastChapter = JsonHelper.ParseInt(token["chapters_len"]);
            manga.LastChapterDate = this.ParseDateTime(JsonHelper.ParseInt(token["last_chapter_date"]));
            manga.LastChapterRead = null;

            manga.Chapters = token["chapters"].Children().Select(c => this.ParseChapter(manga.Key, c)).OrderBy(c => c.Number);
            return manga;
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

        // Working
        private List<string> ParsePages(IEnumerable<JToken> pages)
        {
            return pages
                .Select(t => new { Number = t["number"].Value<int>(), Url = t["url"].Value<string>() })
                .OrderBy(p => p.Number)
                .Select(p => p.Url).ToList();
        }

        // Working
        private MangaStatus? ParseMangaStatus(int? id)
        {
            if (id.HasValue)
            {
                switch (id)
                {
                    case 0:
                        return MangaStatus.Cancelled;
                    case 1:
                        return MangaStatus.Ongoing;
                    case 2:
                        return MangaStatus.Completed;
                }
            }

            return null;
        }

        // Working
        private ReadingDirection? ParseReadingDirection(int? id)
        {
            if (id.HasValue)
            {
                switch (id.Value)
                {
                    case 0:
                        return ReadingDirection.LTR;
                    case 1:
                        return ReadingDirection.RTL;
                }
            }

            return null;
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
