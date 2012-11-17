namespace MangAppClient.Core.Services
{
    using MangAppClient.Core.Model;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    internal static class Requests
    {
        internal static async Task<IEnumerable<MangaSummary>> GetMangaList()
        {
            try
            {
                List<MangaSummary> results = new List<MangaSummary>();

                HttpClient client = new HttpClient();
                var response = await client.GetStringAsync(string.Format(Urls.GetMangaList));

                // Transform JSON into object
                JObject json = JObject.Parse(response);

                results.AddRange(json["manga"].Children().Select(t => ParseMangaSummary(t)));

                return results;
            }
            catch (HttpRequestException e)
            {
                return Enumerable.Empty<MangaSummary>();
            }
        }

        internal static async Task<IEnumerable<DiffResult>> GetMangaListDiff(int localListId)
        {
            try
            {
                List<DiffResult> results = new List<DiffResult>();

                HttpClient client = new HttpClient();
                var response = await client.GetStringAsync(string.Format(Urls.GetMangaDiff, localListId));

                // Transform JSON into object
                JObject json = JObject.Parse(response);
                var groups = json["manga"].Children().GroupBy(t => t["operation"].Value<string>());

                // Get the mangas that were deleted
                results.AddRange(groups
                    .Where(group => group.Key.Equals("delete", StringComparison.CurrentCultureIgnoreCase))
                    .SelectMany(group => group)
                    .Select(item => new RemoveDiffResult(item["id"].Value<int>())));

                // Get the mangas that were updated
                results.AddRange(groups
                    .Where(group => group.Key.Equals("update", StringComparison.CurrentCultureIgnoreCase))
                    .SelectMany(group => group)
                    .Select(item => new UpdateDiffResult(
                        item["id"].Value<int>(),
                        item["chapter"].Value<int>(),
                        string.IsNullOrEmpty(item["status"].Value<string>()) ? null : (MangaStatus?)Enum.Parse(typeof(MangaStatus), item["status"].Value<string>()))));

                // Get the mangas that were added
                results.AddRange(groups
                    .Where(group => group.Key.Equals("add", StringComparison.CurrentCultureIgnoreCase))
                    .SelectMany(group => group)
                    .Select(item => new AddDiffResult(item["id"].Value<int>())
                        {
                            Name = item["name"].Value<string>(),
                            Authors = item["authors"].Children().Values<string>(),
                            Artists = item["artists"].Children().Values<string>(),
                            Genres = item["genres"].Children().Values<string>(),
                            LastChapter = item["chapter"].Value<int>(),
                            Status = (MangaStatus)Enum.Parse(typeof(MangaStatus), item["status"].Value<string>())
                        }));

                return results;
            }
            catch (HttpRequestException e)
            {
                return Enumerable.Empty<DiffResult>();
            }
        }

        internal static async Task<Manga> GetMangaDetail(int mangaId)
        {

        }

        internal static async Task<IEnumerable<MangaSummary>> GetAuthorMangas(string authorId)
        {
            try
            {
                List<MangaSummary> results = new List<MangaSummary>();

                HttpClient client = new HttpClient();
                var response = await client.GetStringAsync(string.Format(Urls.GetAuthorMangas, authorId));

                // Transform JSON into object
                JObject json = JObject.Parse(response);

                results.AddRange(json["manga"].Children().Select(t => ParseMangaSummary(t)));

                return results;
            }
            catch (HttpRequestException e)
            {
                return Enumerable.Empty<MangaSummary>();
            }
        }

        internal static async Task<IEnumerable<MangaSummary>> GetRelatedMangas(int mangaId)
        {
            try
            {
                List<MangaSummary> results = new List<MangaSummary>();

                HttpClient client = new HttpClient();
                var response = await client.GetStringAsync(string.Format(Urls.GetRelatedMangas, mangaId));

                // Transform JSON into object
                JObject json = JObject.Parse(response);

                results.AddRange(json["manga"].Children().Select(t => ParseMangaSummary(t)));

                return results;
            }
            catch (HttpRequestException e)
            {
                return Enumerable.Empty<MangaSummary>();
            }
        }

        private static MangaSummary ParseMangaSummary(JToken token)
        {
            return new MangaSummary()
                    {
                        Id = token["id"].Value<string>(),
                        Name = token["name"].Value<string>()
                    };
        }

        private static Manga ParseManga(JToken token)
        {
            return new Manga()
            {
                Id = token["id"].Value<string>(),
                Name = token["name"].Value<string>(),
                Author = token["author"].Value<string>(),
                Artist = token["artist"].Value<string>(),
                Genre = token["genre"].Value<string>(),
                Status = (MangaStatus) Enum.Parse(typeof(MangaStatus), token["status"].Value<string>())
                // Chapters
            };

        public MangaStatus Status { get; set; }
        public int LastChapter { get; set; }
        }
    }
}
