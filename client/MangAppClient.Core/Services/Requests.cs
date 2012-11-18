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
        internal int MangaListVersion { get; private set ; }

        public async Task<Manga> GetMangaDetailAsync(string mangaId)
        {
            try
            {
                List<MangaSummary> results = new List<MangaSummary>();

                HttpClient client = new HttpClient();
                var response = await client.GetStringAsync(string.Format(Urls.GetMangaDetail, mangaId));

                // Transform JSON into manga
                JObject json = JObject.Parse(response);
                return this.ParseManga(json["manga"]);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<Chapter> GetChapterAsync(string mangaId, int chapterId)
        {
            try
            {
                List<MangaSummary> results = new List<MangaSummary>();

                HttpClient client = new HttpClient();
                var response = await client.GetStringAsync(string.Format(Urls.GetMangaChapter, mangaId, chapterId));

                // Transform JSON into manga
                JObject json = JObject.Parse(response);
                return this.ParseChapter(json["chapter"]);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<Chapter> GetChapterFromProviderAsync(string mangaId, int chapterId, int providerId)
        {
            try
            {
                List<MangaSummary> results = new List<MangaSummary>();

                HttpClient client = new HttpClient();
                var response = await client.GetStringAsync(string.Format(Urls.GetMangaChapterFromProvider, mangaId, chapterId, providerId));

                // Transform JSON into manga
                JObject json = JObject.Parse(response);
                return this.ParseChapter(json["chapter"]);
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<IEnumerable<MangaSummary>> GetAuthorMangasAsync(string authorId)
        {
            try
            {
                List<MangaSummary> results = new List<MangaSummary>();

                HttpClient client = new HttpClient();
                var response = await client.GetStringAsync(string.Format(Urls.GetAuthorMangas, authorId));

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

        public async Task<IEnumerable<MangaSummary>> GetRelatedMangasAsync(string mangaId)
        {
            try
            {
                List<MangaSummary> results = new List<MangaSummary>();

                HttpClient client = new HttpClient();
                var response = await client.GetStringAsync(string.Format(Urls.GetRelatedMangas, mangaId));

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

        public async Task<IEnumerable<int>> GetFavoriteMangasAsync(int userId)
        {
            try
            {
                List<int> results = new List<int>();

                HttpClient client = new HttpClient();
                var response = await client.GetStringAsync(string.Format(Urls.GetFavoriteMangas, userId));

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

        internal async Task<IEnumerable<MangaSummary>> GetMangaListAsync()
        {
            try
            {
                List<MangaSummary> results = new List<MangaSummary>();

                HttpClient client = new HttpClient();
                var response = await client.GetStringAsync(Urls.GetMangaList);

                // Transform JSON into objects
                JArray json = JArray.Parse(response);

                this.MangaListVersion = 1; // json["version"].Value<int>();
                results.AddRange(json.Children().Select(t => this.ParseMangaSummary(t)));

                return results;
            }
            catch (HttpRequestException ex)
            {
                return Enumerable.Empty<MangaSummary>();
            }
        }

        internal async Task<IEnumerable<DiffResult>> GetMangaListDiffAsync(int localListVersion)
        {
            try
            {
                List<DiffResult> results = new List<DiffResult>();

                HttpClient client = new HttpClient();
                var response = await client.GetStringAsync(string.Format(Urls.GetMangaDiff, localListVersion));

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

        internal async Task<byte[]> GetBackgroundImageAsync(string mangaId)
        {
            try
            {
                HttpClient client = new HttpClient();
                return await client.GetByteArrayAsync(string.Format(Urls.GetBackgroundImage, mangaId));
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        private MangaSummary ParseMangaSummary(JToken token)
        {
            return new MangaSummary(token["id"].Value<string>())
                    {
                        Title = token["name"].Value<string>(),
                        Authors = token["authors"].Children().Values<string>(),
                        Artists = token["artists"].Children().Values<string>(),
                        Categories = token["genres"].Children().Values<string>(),
                        LastChapter = token["chapter"].Value<int>(),
                        Status = (MangaStatus)Enum.Parse(typeof(MangaStatus), token["status"].Value<string>())
                    };
        }

        private Manga ParseManga(JToken token)
        {
            return new Manga()
                    {
                        Id = token["id"].Value<string>(),
                        Name = token["name"].Value<string>(),
                        AlternativeNames = token["alternativeNames"].Children().Values<string>(),
                        Description = token["description"].Value<string>(),
                        Providers = token["providers"].Children().Values<string>(),
                        Author = token["authors"].Children().Values<string>(),
                        Artist = token["artists"].Children().Values<string>(),
                        Genre = token["genres"].Children().Values<string>(),
                        Status = (MangaStatus) Enum.Parse(typeof(MangaStatus), token["status"].Value<string>()),
                        Year = token["year"].Value<int>(),
                        TotalChapters = token["totalChapters"].Value<int>(),
                        Image = new Uri(token["image"].Value<string>()),
                        LastChapters = token["chapters"].Children().Select(c => this.ParseChapterSummary(c))
                    };
        }

        private ChapterSummary ParseChapterSummary(JToken token)
        {
            return new ChapterSummary()
                    {
                        Id = token["id"].Value<int>(),
                        Number = token["number"].Value<int>(),
                        Title = token["title"].Value<string>()
                    };
        }

        private Chapter ParseChapter(JToken token)
        {
            return new Chapter()
            {
                Id = token["id"].Value<int>(),
                PreviousChapterId = token["previous"].Value<int>(),
                NextChapterId = token["next"].Value<int>(),
                Number = token["number"].Value<int>(),
                Title = token["title"].Value<string>(),
                Pages = token["pages"].Children().Values<string>().Select(s => new Uri(s))
            };
        }
    }
}
