namespace MangAppClient.Core.Services
{
    using MangAppClient.Core.Model;
    using MangAppClient.Core.Utilities;
    using SQLite;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Windows.ApplicationModel;
    using Windows.Storage;
    using Windows.Storage.Search;
    using Windows.UI.Xaml.Media.Imaging;

    public class LocalData : ILocalData
    {
        private ObservableCollection<Manga> mangaList;

        private Random random = new Random();

        public ObservableCollection<Manga> MangaList
        {
            get { return this.mangaList; }
        }

        // Working
        public IEnumerable<Manga> GetMangaList()
        {
            using (SQLiteConnection db = new SQLiteConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "mangapp.db")))
            {
                return db.Table<Manga>().ToList().OrderByDescending(m => m.Popularity);
            }
        }

        public async void UpdateMangaList()
        {
            //SQLiteAsyncConnection db = new SQLiteAsyncConnection("mangapp.db");
            //DbMangaListVersion listVersion = await db.Table<DbMangaListVersion>().FirstOrDefaultAsync();

            //Requests requests = new Requests();
            //var diffs = requests.GetMangaListDiff(listVersion.Version);

            //// Update our local manga list version
            //listVersion.Version = requests.MangaListVersion;
            //await db.UpdateAsync(listVersion);

            //// Update our local manga list with the diffs from the server
            //// Remove old mangas
            //foreach (var deletion in diffs.OfType<RemoveDiffResult>())
            //{
            //    await db.DeleteAsync<DbMangaSummary>(deletion.Id);
            //}

            //// Update mangas
            //var updates = diffs.OfType<UpdateDiffResult>();
            //var dbMangas = await db.Table<DbMangaSummary>()
            //            .Where(m => updates.Any(u => u.Id == m.Key))
            //            .ToListAsync();

            //var updatedDbMangas = updates.Join(
            //    dbMangas,
            //    update => update.Id,
            //    dbManga => dbManga.Key,
            //    (update, dbManga) => dbManga.Update(update));

            //await db.UpdateAsync(updatedDbMangas);

            //// Insert new mangas
            //await db.InsertAllAsync(diffs.OfType<MangaSummary>().Select(m => DbMangaSummary.FromMangaSummary(m)));
        }

        public void AddFavoriteManga(Manga manga)
        {
            using (SQLiteConnection db = new SQLiteConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "mangapp.db")))
            {
                manga.LastChapterRead = 0;
                db.Update(manga);
            }
        }

        public void RemoveFavoriteManga(Manga manga)
        {
            using (SQLiteConnection db = new SQLiteConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "mangapp.db")))
            {
                manga.LastChapterRead = null;
                db.Update(manga);
            }
        }

        public void UpdateFavoriteManga(Manga manga, int lastChapterRead)
        {
            using (SQLiteConnection db = new SQLiteConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "mangapp.db")))
            {
                manga.LastChapterRead = lastChapterRead;
                db.Update(manga);
            }
        }

        public string GetBackgroundImage(Manga manga)
        {
            var folder = FileSystemUtilities.GetFolder(ApplicationData.Current.LocalFolder, Constants.BackgroundImagesFolderPath);
            if (folder != null)
            {
                // Let's search first by manga key
                var defaultFiles = folder.GetFilesAsync().AsTask().Result
                    .Where(f => f.Name.Contains(manga.Key))
                    .ToList();

                if (defaultFiles.Count > 1)
                {
                    return defaultFiles[random.Next(0, defaultFiles.Count)].Path;
                }

                // Let's search first by manga name
                defaultFiles = folder.GetFilesAsync().AsTask().Result
                    .Where(f => f.Name.Contains(manga.Title))
                    .ToList();

                if (defaultFiles.Count > 1)
                {
                    return defaultFiles[random.Next(0, defaultFiles.Count)].Path;
                }
            }

            return null;
        }

        public string GetDefaultBackgroundImage()
        {
            var folder = FileSystemUtilities.GetFolder(ApplicationData.Current.LocalFolder, Constants.BackgroundImagesFolderPath);
            if (folder != null)
            {
                var defaultFiles = folder.GetFilesAsync().AsTask().Result
                    .Where(f => f.Name.Contains("default"))
                    .ToList();

                return defaultFiles[random.Next(0, defaultFiles.Count)].Path;
            }

            return Path.Combine(Constants.BackgroundImagesFolderPath, "default.jpg");
        }

        public string UpdateBackgroundImage(Manga manga)
        {
            byte[] imageData = new WebData().GetBackgroundImage(manga.Key);

            if (imageData != null && imageData.Length > 0)
            {
                string fileName = manga.Key + ".jpg";
                var file = ApplicationData.Current.LocalFolder.CreateFileAsync(Path.Combine(Constants.BackgroundImagesFolderPath, fileName), Windows.Storage.CreationCollisionOption.ReplaceExisting).AsTask().Result;

                using (var stream = file.OpenStreamForWriteAsync().Result)
                {
                    stream.Write(imageData, 0, imageData.Length);
                }

                return fileName;
            }

            return null;
        }

        // Working
        private void CreateSummaryImage(HttpClient client, Manga manga)
        {
            try
            {
                byte[] imageData = client.GetByteArrayAsync(manga.RemoteSummaryImage).Result;
                if (imageData != null && imageData.Length > 0)
                {
                    string fileName = manga.Key + Path.GetExtension(manga.RemoteSummaryImageDb);
                    var file = ApplicationData.Current.LocalFolder.CreateFileAsync(Path.Combine(Constants.SummaryImagesFolderPath, fileName), CreationCollisionOption.ReplaceExisting).AsTask().Result;

                    using (var stream = file.OpenStreamForWriteAsync().Result)
                    {
                        stream.Write(imageData, 0, imageData.Length);
                    }

                    manga.LocalSummaryImage = Path.Combine(Constants.SummaryImagesFolderPath, manga.Key + Path.GetExtension(manga.RemoteSummaryImageDb));
                }
            }
            catch (Exception)
            {
                // No image in the server, let's use a random default one
                var folder = FileSystemUtilities.GetFolder(ApplicationData.Current.LocalFolder, Constants.SummaryImagesFolderPath);
                if (folder != null)
                {
                    var defaultFiles = folder.GetFilesAsync().AsTask().Result
                        .Where(f => f.Name.Contains("default"))
                        .ToList();

                    manga.LocalSummaryImage = defaultFiles[random.Next(0, defaultFiles.Count)].Path;
                }
                else
                {
                    manga.LocalSummaryImage = null;
                }
            }
        }
    }
}
