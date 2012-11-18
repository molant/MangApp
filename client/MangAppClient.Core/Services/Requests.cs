namespace MangAppClient.Core.Services
{
    using MangAppClient.Core.Model;
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
        public Manga GetMangaDetail(string mangaId)
        {
            try
            {
                List<MangaSummary> results = new List<MangaSummary>();

                HttpClient client = new HttpClient();
                var response = client.GetStringAsync(string.Format(Urls.GetMangaDetail, mangaId)).Result;

                // Transform JSON into manga
                JObject json = JObject.Parse(response);
                return this.ParseManga(json);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Chapter GetChapter(string mangaId, int chapterId)
        {
            try
            {
                List<MangaSummary> results = new List<MangaSummary>();

                HttpClient client = new HttpClient();
                var response = client.GetStringAsync(string.Format(Urls.GetMangaChapter, mangaId, chapterId)).Result;

                // Transform JSON into manga
                JObject json = JObject.Parse(response);
                return this.ParseChapter(json["chapter"]);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public Chapter GetChapterFromProvider(string mangaId, int chapterId, int providerId)
        {
            try
            {
                List<MangaSummary> results = new List<MangaSummary>();

                HttpClient client = new HttpClient();
                var response = client.GetStringAsync(string.Format(Urls.GetMangaChapterFromProvider, mangaId, chapterId, providerId)).Result;

                // Transform JSON into manga
                JObject json = JObject.Parse(response);
                return this.ParseChapter(json["chapter"]);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public IEnumerable<MangaSummary> GetAuthorMangas(string authorId)
        {
            try
            {
                List<MangaSummary> results = new List<MangaSummary>();

                HttpClient client = new HttpClient();
                var response = client.GetStringAsync(string.Format(Urls.GetAuthorMangas, authorId)).Result;

                // Transform JSON into objects
                JObject json = JObject.Parse(response);
                results.AddRange(json["manga"].Children().Select(t => this.ParseMangaSummary(t)));

                return results;
            }
            catch (HttpRequestException)
            {
                return Enumerable.Empty<MangaSummary>();
            }
        }

        public IEnumerable<MangaSummary> GetRelatedMangas(string mangaId)
        {
            try
            {
                List<MangaSummary> results = new List<MangaSummary>();

                HttpClient client = new HttpClient();
                var response = client.GetStringAsync(string.Format(Urls.GetRelatedMangas, mangaId)).Result;

                // Transform JSON into objects
                JObject json = JObject.Parse(response);
                results.AddRange(json["manga"].Children().Select(t => this.ParseMangaSummary(t)));

                return results;
            }
            catch (HttpRequestException)
            {
                return Enumerable.Empty<MangaSummary>();
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
        internal IEnumerable<MangaSummary> GetMangaList()
        {
            try
            {
                List<MangaSummary> results = new List<MangaSummary>();

                HttpClient client = new HttpClient();
                var response = client.GetStringAsync(Urls.GetMangaList).Result;

                // Transform JSON into objects
                JArray json = JArray.Parse(response);

                this.MangaListVersion = 1; // json["version"].Value<int>();
                results.AddRange(json.Children().Select(t => this.ParseMangaSummary(t)));

                return results;
            }
            catch (HttpRequestException)
            {
                return Enumerable.Empty<MangaSummary>();
            }
        }

        internal IEnumerable<DiffResult> GetMangaListDiff(int localListVersion)
        {
            try
            {
                List<DiffResult> results = new List<DiffResult>();

                HttpClient client = new HttpClient();
                var response = client.GetStringAsync(string.Format(Urls.GetMangaDiff, localListVersion)).Result;

                // Transform JSON into object
                JObject json = JObject.Parse(response);

                this.MangaListVersion = json["version"].Value<int>();
                var groups = json["manga"].Children().GroupBy(t => t["operation"].Value<string>());

                // Get the mangas that were deleted
                results.AddRange(groups
                    .Where(group => group.Key.Equals("delete", StringComparison.CurrentCultureIgnoreCase))
                    .SelectMany(group => group)
                    .Select(item => new RemoveDiffResult(item["id"].Value<string>())));

                // Get the mangas that were updated
                results.AddRange(groups
                    .Where(group => group.Key.Equals("update", StringComparison.CurrentCultureIgnoreCase))
                    .SelectMany(group => group)
                    .Select(item => new UpdateDiffResult(
                        item["id"].Value<string>(),
                        item["chapter"].Value<int>(),
                        string.IsNullOrEmpty(item["status"].Value<string>()) ? null : (MangaStatus?)Enum.Parse(typeof(MangaStatus), item["status"].Value<string>()))));

                // Get the mangas that were added
                results.AddRange(groups
                    .Where(group => group.Key.Equals("add", StringComparison.CurrentCultureIgnoreCase))
                    .SelectMany(group => group)
                    .Select(item => this.ParseMangaSummary(item)));

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
        private MangaSummary ParseMangaSummary(JToken token)
        {
            MangaSummary manga = new MangaSummary(token["_id"].Value<string>());

            manga.Title = token["title"].Value<string>();
            manga.Description = token["description"].Value<string>();
            manga.AlternativeNames = token["alias"].Children().Values<string>();

            manga.Authors = token["authors"].Children().Values<string>();
            manga.Artists = token["artists"].Children().Values<string>();
            manga.Categories = token["categories"].Children().Values<string>();

            manga.YearOfRelease = this.ParseYear(token["released"]);
            manga.Status = this.ParseMangaStatus(token["status"].Value<int>());
            manga.ReadingDirection = this.ParseReadingDirection(token["direction"]);
            manga.SummaryImageUrl = new Uri(token["image"].Value<string>());

            manga.LastChapter = token["chapters_len"].Value<int>();
            manga.LastChapterDate = this.ParseDateTime(token["last_chapter_date"]);

            return manga;
        }

        // Working
        private Manga ParseManga(JToken token)
        {
            Manga manga = new Manga();

            manga.Id = token["_id"].Value<string>(); 
            manga.Title = token["title"].Value<string>();
            manga.Description = token["description"].Value<string>();
            manga.AlternativeNames = token["alias"].Children().Values<string>();

            //manga.Providers = token["providers"].Children().Values<string>();

            manga.Authors = token["authors"].Children().Values<string>();
            manga.Artists = token["artists"].Children().Values<string>();
            manga.Categories = token["categories"].Children().Values<string>();

            manga.YearOfRelease = this.ParseYear(token["released"]);
            manga.Status = this.ParseMangaStatus(token["status"].Value<int>());
            manga.ReadingDirection = this.ParseReadingDirection(token["direction"]);
            manga.SummaryImageUrl = new Uri(token["image"].Value<string>());

            manga.LastChapter = token["chapters_len"].Value<int>();
            manga.LastChapterDate = this.ParseDateTime(token["last_chapter_date"]);

            manga.Chapters = token["chapters"].Children().Select(c => this.ParseChapterSummary(c)).OrderBy(c => c.Number);
            return manga;
        }

        // Working
        private ChapterSummary ParseChapterSummary(JToken token)
        {
            ChapterSummary chapterSummary = new ChapterSummary();

            chapterSummary.Id = token["_id"].Value<string>();
            chapterSummary.Number = token["number"].Value<int>();
            chapterSummary.Title = token["title"].Value<string>();
            chapterSummary.UploadedDate = this.ParseDateTime(token["uploadedDate"]);

            return chapterSummary;
        }

        private Chapter ParseChapter(JToken token)
        {
            return new Chapter()
            {
                Id = token["_id"].Value<string>(),
                PreviousChapterId = token["previous"].Value<string>(),
                NextChapterId = token["next"].Value<string>(),
                Number = token["number"].Value<int>(),
                Title = token["title"].Value<string>(),
                Pages = token["pages"].Children().Values<string>().Select(s => new Uri(s))
            };
        }

        // Working
        private MangaStatus ParseMangaStatus(int id)
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

            return MangaStatus.Ongoing;
        }

        // Working
        private ReadingDirection? ParseReadingDirection(JToken id)
        {
            if (id != null)
            {
                int? d = id.Value<int?>();
                if (d.HasValue)
                {
                    switch (d.Value)
                    {
                        case 0:
                            return ReadingDirection.LTR;

                        case 1:
                            return ReadingDirection.RTL;
                    }
                }
            }

            return null;
        }

        // Working
        private DateTime? ParseDateTime(JToken days)
        {
            if (days != null)
            {
                int? d = days.Value<int?>();
                if (d.HasValue)
                {
                    DateTime dateTime = new DateTime();
                    return dateTime.AddDays(d.Value);
                }
            }

            return null;
        }

        // Working
        private int? ParseYear(JToken days)
        {
            if (days != null)
            {
                int? d = days.Value<int?>();
                if (d.HasValue)
                {
                    DateTime dateTime = new DateTime();
                    return dateTime.AddDays(d.Value).Year;
                }
            }

            return null;
        }
    }
}
