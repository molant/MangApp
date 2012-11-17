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
        internal static async IEnumerable<object> FetchMangaListDiff()
        {
            try
            {
                HttpClient client = new HttpClient();
                var json = await client.GetStringAsync(Urls.GetMangaList);

                // Transform JSON into object
            }
            catch (HttpRequestException e)
            {
            }
        }
    }
}
