namespace MangAppClient.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    internal static class Requests
    {
        internal static async Task<IEnumerable<DiffResult>> FetchMangaListDiff(int localListId)
        {
            try
            {
                HttpClient client = new HttpClient();
                var json = await client.GetStringAsync(string.Format(Urls.GetMangaDiff, localListId));

                // Transform JSON into object
                return Enumerable.Empty<DiffResult>();
            }
            catch (HttpRequestException e)
            {
                return Enumerable.Empty<DiffResult>();
            }
        }
    }
}
