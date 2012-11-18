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
    using Windows.UI.Xaml.Media.Imaging;

    public class Database : IDatabase
    {
        private static readonly string[] Separators = { "#" };

        private static readonly string SummaryImagesFolder = "SummaryImages";

        private static readonly string BackgroundImagesFolder = "BackgroundImages";

        // Working
        public IEnumerable<MangaSummary> GetMangaList()
        {
            using (SQLiteConnection db = new SQLiteConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "mangapp.db")))
            {
                var mangas = db.Table<DbMangaSummary>().ToList();
                return mangas.Select(m => DbMangaSummary.ToMangaSummary(m));
            }
        }

        public async void UpdateMangaList()
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection("mangapp.db");
            DbMangaListVersion listVersion = await db.Table<DbMangaListVersion>().FirstOrDefaultAsync();

            Requests requests = new Requests();
            var diffs = requests.GetMangaListDiff(listVersion.Version);

            // Update our local manga list version
            listVersion.Version = requests.MangaListVersion;
            await db.UpdateAsync(listVersion);

            // Update our local manga list with the diffs from the server
            // Remove old mangas
            foreach (var deletion in diffs.OfType<RemoveDiffResult>())
            {
                await db.DeleteAsync<DbMangaSummary>(deletion.Id);
            }

            // Update mangas
            var updates = diffs.OfType<UpdateDiffResult>();
            var dbMangas = await db.Table<DbMangaSummary>()
                        .Where(m => updates.Any(u => u.Id == m.Key))
                        .ToListAsync();

            var updatedDbMangas = updates.Join(
                dbMangas,
                update => update.Id,
                dbManga => dbManga.Key,
                (update, dbManga) => dbManga.Update(update));

            await db.UpdateAsync(updatedDbMangas);

            // Insert new mangas
            await db.InsertAllAsync(diffs.OfType<MangaSummary>().Select(m => DbMangaSummary.FromMangaSummary(m)));
        }

        // Working
        public async void CreateInitialDb()
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
            var installFolder = Package.Current.InstalledLocation.GetFolderAsync(BackgroundImagesFolder).AsTask().Result;
            foreach (var file in installFolder.GetFilesAsync().AsTask().Result)
            {
                var copiedFile = file.CopyAsync(backgroundFolder, file.Name, NameCollisionOption.ReplaceExisting).AsTask().Result;
            }

            // Populate the manga list from the server information
            IEnumerable<MangaSummary> mangas;
            Requests requests = new Requests();
            mangas = requests.GetMangaList().OrderByDescending(m => m.Popularity);

            // Get additional summary and background images from the server
            HttpClient client = new HttpClient();
            foreach (var manga in mangas)
            {
                this.CreateSummaryImage(client, manga);
                this.UpdateBackgroundImage(manga.Id);
            }

            // Add mangas to the database
            using (SQLiteConnection db = new SQLiteConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "mangapp.db")))
            {
                db.CreateTable<DbMangaListVersion>();
                db.CreateTable<DbMangaSummary>();

                db.Insert(new DbMangaListVersion(requests.MangaListVersion));
                db.InsertAll(mangas.Select(m => DbMangaSummary.FromMangaSummary(m)));
            }
        }

        // TODO: add selection of a random default image
        public BitmapImage GetDefaultBackgroundImage()
        {
            return new BitmapImage(new Uri(Path.Combine(BackgroundImagesFolder, "default.jpg")));
        }

        // TODO: add check by name and by id to search for a background image
        public BitmapImage GetBackgroundImage(string mangaId)
        {
            DbMangaSummary summary;
            using (SQLiteConnection db = new SQLiteConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "mangapp.db")))
            {
                summary = db.Table<DbMangaSummary>().Where(m => m.Key == mangaId).FirstOrDefault();
            }

            if (summary != null)
            {
                string imagePath = Path.Combine(BackgroundImagesFolder, summary.Title + ".jpg");

                var file = this.FileExits(ApplicationData.Current.LocalFolder, imagePath);
                if (file != null)
                {
                    return new BitmapImage(new Uri(file.Path));
                }
            }

            return null;
        }

        public BitmapImage UpdateBackgroundImage(string mangaId)
        {
            byte[] imageData = new Requests().GetBackgroundImage(mangaId);

            if (imageData != null && imageData.Length > 0)
            {
                string fileName = mangaId + ".jpg";
                var file = ApplicationData.Current.LocalFolder.CreateFileAsync(Path.Combine(BackgroundImagesFolder, fileName), Windows.Storage.CreationCollisionOption.ReplaceExisting).AsTask().Result;

                using (var stream = file.OpenStreamForWriteAsync().Result)
                {
                    stream.Write(imageData, 0, imageData.Length);
                }

                return new BitmapImage(new Uri(fileName));
            }

            return null;
        }

        // Working
        private void CreateSummaryImage(HttpClient client, MangaSummary manga)
        {
            try
            {
                byte[] imageData = client.GetByteArrayAsync(manga.SummaryImageUrl).Result;
                if (imageData != null && imageData.Length > 0)
                {
                    string extension = Path.GetExtension(manga.SummaryImageUrl.ToString());
                    string fileName = manga.Id + extension;

                    var file = ApplicationData.Current.LocalFolder.CreateFileAsync(Path.Combine(SummaryImagesFolder, fileName), CreationCollisionOption.ReplaceExisting).AsTask().Result;
                    using (var stream = file.OpenStreamForWriteAsync().Result)
                    {
                        stream.Write(imageData, 0, imageData.Length);
                    }
                }
            }
            catch (Exception)
            {
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

        private class DbMangaListVersion
        {
            public DbMangaListVersion() { }

            public DbMangaListVersion(int version)
            {
                this.Version = version;
            }

            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }

            public int Version { get; set; }
        }

        private class DbMangaSummary
        {
            public string Key { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string AlternativeNames { get; set; }
            public int Popularity { get; set; }

            public string Authors { get; set; }
            public string Artists { get; set; }
            public string Categories { get; set; }

            public int? YearOfRelease { get; set; }
            public int Status { get; set; }
            public int? ReadingDirection { get; set; }
            public string SummaryImageUrl { get; set; }

            public int LastChapter { get; set; }
            public DateTime? LastChapterDate { get; set; }

            public DbMangaSummary Update(UpdateDiffResult update)
            {
                this.LastChapter = update.LastChapter;
                if (update.NewStatus.HasValue)
                {
                    this.Status = (int)update.NewStatus.Value;
                }

                return this;
            }

            public static DbMangaSummary FromMangaSummary(MangaSummary summary)
            {
                return new DbMangaSummary()
                        {
                            Key = summary.Id,
                            Title = summary.Title,
                            Description = summary.Description,
                            AlternativeNames = string.Join(Database.Separators[0], summary.AlternativeNames),
                            Popularity = summary.Popularity,

                            Authors = string.Join(Database.Separators[0], summary.Authors),
                            Artists = string.Join(Database.Separators[0], summary.Artists),
                            Categories = string.Join(Database.Separators[0], summary.Categories),

                            YearOfRelease = summary.YearOfRelease,
                            Status = (int)summary.Status,
                            ReadingDirection = (int?)summary.ReadingDirection,
                            SummaryImageUrl = summary.SummaryImageUrl,

                            LastChapter = summary.LastChapter,
                            LastChapterDate = summary.LastChapterDate
                        };
            }

            public static MangaSummary ToMangaSummary(DbMangaSummary dbManga)
            {
                return new MangaSummary(dbManga.Key)
                        {
                            Title = dbManga.Title,
                            Description = dbManga.Description,
                            AlternativeNames = dbManga.AlternativeNames.Split(Database.Separators, StringSplitOptions.RemoveEmptyEntries),
                            Popularity = dbManga.Popularity,

                            Authors = dbManga.Authors.Split(Database.Separators, StringSplitOptions.RemoveEmptyEntries),
                            Artists = dbManga.Artists.Split(Database.Separators, StringSplitOptions.RemoveEmptyEntries),
                            Categories = dbManga.Categories.Split(Database.Separators, StringSplitOptions.RemoveEmptyEntries),

                            YearOfRelease = dbManga.YearOfRelease,
                            Status = (MangaStatus)dbManga.Status,
                            ReadingDirection = (ReadingDirection?)dbManga.ReadingDirection,
                            SummaryImageUrl = dbManga.SummaryImageUrl,

                            LastChapter = dbManga.LastChapter,
                            LastChapterDate = dbManga.LastChapterDate
                        };
            }
        }
    }
}
