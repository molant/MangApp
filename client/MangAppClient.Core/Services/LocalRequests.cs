namespace MangAppClient.Core.Services
{
    using MangAppClient.Core.Model;
    using SQLite;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Windows.ApplicationModel;
    using Windows.Storage;
    using Windows.Storage.Search;
    using Windows.UI.Xaml.Media.Imaging;

    public class LocalRequests : ILocalRequests
    {
        private static readonly string[] Separators = { "#" };

        private static readonly string SummaryImagesFolder = "SummaryImages";

        private static readonly string BackgroundImagesFolder = "BackgroundImages";

        private static Random random = new Random();

        // Working
        public void CreateInitialDb()
        {
            // Recreate the local files and folders
            var dbFile = this.FileExits(ApplicationData.Current.LocalFolder, "mangapp.db");
            if (dbFile != null)
            {
                dbFile.DeleteAsync().AsTask().Wait();
            }

            var summaryFolder = this.FolderExists(ApplicationData.Current.LocalFolder, SummaryImagesFolder);
            if (summaryFolder != null)
            {
                summaryFolder.DeleteAsync().AsTask().Wait();
            }

            var backgroundFolder = this.FolderExists(ApplicationData.Current.LocalFolder, BackgroundImagesFolder);
            if (backgroundFolder != null)
            {
                backgroundFolder.DeleteAsync().AsTask().Wait();
            }

            summaryFolder = ApplicationData.Current.LocalFolder.CreateFolderAsync(SummaryImagesFolder, CreationCollisionOption.ReplaceExisting).AsTask().Result;
            backgroundFolder = ApplicationData.Current.LocalFolder.CreateFolderAsync(BackgroundImagesFolder, CreationCollisionOption.ReplaceExisting).AsTask().Result;

            // Copy the background images from the installed folder to the app folder
            var backgroundInstallFolder = Package.Current.InstalledLocation.GetFolderAsync(BackgroundImagesFolder).AsTask().Result;
            foreach (var file in backgroundInstallFolder.GetFilesAsync().AsTask().Result)
            {
                var copiedFile = file.CopyAsync(backgroundFolder, file.Name, NameCollisionOption.ReplaceExisting).AsTask().Result;
            }

            // Copy the summary images from the installed folder to the app folder
            var summaryInstallFolder = Package.Current.InstalledLocation.GetFolderAsync(SummaryImagesFolder).AsTask().Result;
            foreach (var file in summaryInstallFolder.GetFilesAsync().AsTask().Result)
            {
                var copiedFile = file.CopyAsync(summaryFolder, file.Name, NameCollisionOption.ReplaceExisting).AsTask().Result;
            }

            // Populate the manga list from the server information
            WebRequests requests = new WebRequests();
            IEnumerable<Manga> mangas = requests.GetMangaList();

            // Get additional summary and background images from the server
            HttpClient client = new HttpClient();
            foreach (var manga in mangas)
            {
                this.CreateSummaryImage(client, manga);
                this.UpdateBackgroundImage(manga);
            }

            // Add mangas to the database
            using (SQLiteConnection db = new SQLiteConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "mangapp.db")))
            {
                db.CreateTable<LocalDataVersion>();
                db.CreateTable<Manga>();

                db.Insert(new LocalDataVersion(requests.MangaListVersion));
                db.InsertAll(mangas);
            }
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
            var folder = this.FolderExists(ApplicationData.Current.LocalFolder, BackgroundImagesFolder);
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
            var folder = this.FolderExists(ApplicationData.Current.LocalFolder, BackgroundImagesFolder);
            if (folder != null)
            {
                var defaultFiles = folder.GetFilesAsync().AsTask().Result
                    .Where(f => f.Name.Contains("default"))
                    .ToList();

                return defaultFiles[random.Next(0, defaultFiles.Count)].Path;
            }

            return Path.Combine(BackgroundImagesFolder, "default.jpg");
        }

        public string UpdateBackgroundImage(Manga manga)
        {
            byte[] imageData = new WebRequests().GetBackgroundImage(manga.Key);

            if (imageData != null && imageData.Length > 0)
            {
                string fileName = manga.Key + ".jpg";
                var file = ApplicationData.Current.LocalFolder.CreateFileAsync(Path.Combine(BackgroundImagesFolder, fileName), Windows.Storage.CreationCollisionOption.ReplaceExisting).AsTask().Result;

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
                    var file = ApplicationData.Current.LocalFolder.CreateFileAsync(Path.Combine(SummaryImagesFolder, fileName), CreationCollisionOption.ReplaceExisting).AsTask().Result;

                    using (var stream = file.OpenStreamForWriteAsync().Result)
                    {
                        stream.Write(imageData, 0, imageData.Length);
                    }

                    manga.LocalSummaryImage = Path.Combine(SummaryImagesFolder, manga.Key + Path.GetExtension(manga.RemoteSummaryImageDb));
                }
            }
            catch (Exception)
            {
                // No image in the server, let's use a random default one
                var folder = this.FolderExists(ApplicationData.Current.LocalFolder, SummaryImagesFolder);
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

        // Working
        private StorageFile FileExits(StorageFolder folder, string fileName)
        {
            try
            {
                return folder.GetFileAsync(fileName).AsTask().Result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Working
        private StorageFolder FolderExists(StorageFolder folder, string fileName)
        {
            try
            {
                return folder.GetFolderAsync(fileName).AsTask().Result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
