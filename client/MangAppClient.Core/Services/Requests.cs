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
        internal static async Task<IEnumerable<DiffResult>> FetchMangaListDiff(int localListId)
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
                        string.IsNullOrEmpty(item["status"].Value<string>()) ? null : (MangaStatus?) Enum.Parse(typeof(MangaStatus), item["status"].Value<string>()))));

                // Get the mangas that were added
                results.AddRange(groups
                    .Where(group => group.Key.Equals("add", StringComparison.CurrentCultureIgnoreCase))
                    .SelectMany(group => group)
                    .Select(item => new AddDiffResult(item["id"].Value<int>())
                        {
                            Name = item["name"].Value<string>(),
                            Authors = item["authors"].Children().Select(c => c["name"].Value<string>()),
                            Artists = item["artists"].Children().Select(c => c["name"].Value<string>()),
                            Genres = item["genres"].Children().Values<string>(),
                            LastChapter = item["chapter"].Value<int>(),
                            Status = (MangaStatus) Enum.Parse(typeof(MangaStatus), item["status"].Value<string>())
                        }));

                return results;
            }
            catch (HttpRequestException e)
            {
                return Enumerable.Empty<DiffResult>();
            }
        }
    }
}
