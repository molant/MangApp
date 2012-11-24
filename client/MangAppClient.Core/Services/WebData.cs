namespace MangAppClient.Core.Services
{
    using MangAppClient.Core.Model;
    using MangAppClient.Core.Utilities;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Windows.Data.Json;
    using Windows.Storage;

    public class WebData : IWebData
    {
        internal int MangaListVersion { get; private set; }

        public void GetMangaChapters(Manga manga)
        {
            try
            {
                HttpClient client = new HttpClient();
                var response = client.GetStringAsync(string.Format(Urls.GetMangaDetail, manga.Key)).Result;

                // Transform JSON into manga
                JsonHelper.ParseMangaChapters(manga, JObject.Parse(response));
            }
            catch (Exception)
            {
            }
        }

        public void GetChapterPages(Chapter chapter)
        {
            try
            {
                HttpClient client = new HttpClient();
                var response = client.GetStringAsync(string.Format(Urls.GetMangaChapter, chapter.MangaKey, chapter.Key)).Result;

                // Transform JSON into chapter
                JsonHelper.ParseChapterPages(chapter, JObject.Parse(response));
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
                JsonHelper.ParseChapterPages(chapter, JObject.Parse(response));
            }
            catch (HttpRequestException)
            {
            }
        }

        public async void DownloadMangaChapter(Chapter chapter)
        {
            if (chapter.Pages == null)
            {
                this.GetChapterPages(chapter);
                if (chapter.Pages == null)
                {
                    return;
                }
            }

            StorageFolder folder = FileSystemUtilities.GetFolder(ApplicationData.Current.LocalFolder, Constants.DownloadsFolderPath);

            if (folder != null)
            {
                // TODO: CREATE A NAME IN A WAY THAT WE CAN GET INFORMATION FROM IT!!! (SOMETHING LIKE MANGANAME_CHAPTERNUMBER)
                var file = await folder.CreateFileAsync(chapter.Key, CreationCollisionOption.ReplaceExisting);
                using (var archiveStream = await file.OpenStreamForWriteAsync())
                {
                    ZipArchive archive = new ZipArchive(archiveStream);

                    HttpClient client = new HttpClient();
                    for (int i = 0; i < chapter.Pages.Count; i++)
                    {
                        var data = await client.GetByteArrayAsync(chapter.Pages[i]);

                        var entry = archive.CreateEntry(Path.Combine("i + 1", Path.GetExtension(chapter.Pages[i])));
                        using (var stream = entry.Open())
                        {
                            stream.Write(data, 0, data.Length);
                        }
                    }
                }
            }
        }

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
                results.AddRange(json["mangas"].Children().Select(t => JsonHelper.ParseManga(t)));

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

        internal async Task<IEnumerable<RemoteImage>> GetBackgroundImages(Manga manga)
        {
            try
            {
                HttpClient client = new HttpClient();
                var response = await client.GetStringAsync(Urls.GetMangaList);

                // Transform JSON into objects
                JObject json = JObject.Parse(response);

                // TODO: how are we going to send binary data?
                return null;
            }
            catch (HttpRequestException)
            {
                return Enumerable.Empty<RemoteImage>();
            }
        }

        internal Task<IEnumerable<RemoteImage>> GetDefaultBackgroundImages()
        {
            throw new NotImplementedException();
        }

        internal Task<IEnumerable<RemoteImage>> GetSummaryImages(Manga manga)
        {
            throw new NotImplementedException();
        }

        internal Task<IEnumerable<RemoteImage>> GetDefaultSummaryImages()
        {
            throw new NotImplementedException();
        }

        

        internal byte[] GetBackgroundImage(string mangaId)
        {
            try
            {
                return null;

                HttpClient client = new HttpClient();
                return client.GetByteArrayAsync(string.Format(Urls.GetBackgroundImages, mangaId)).Result;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
    }
}
