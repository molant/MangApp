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

        public BitmapImage GetDefaultBackgroundImage()
        {
            return new BitmapImage(new Uri(Path.Combine(BackgroundImagesFolder, "defaultMangaBackground.png")));
        }

        public BitmapImage GetBackgroundImage(string mangaId)
        {
            var file = ApplicationData.Current.LocalFolder.GetFileAsync(Path.Combine(BackgroundImagesFolder, mangaId + ".jpg")).GetResults();

            if (file != null)
            {
                return new BitmapImage(new Uri(file.Path));
            }

            return null;
        }

        public BitmapImage UpdateBackgroundImage(string mangaId)
        {
            byte[] imageData = new Requests().GetBackgroundImage(mangaId);

            if (imageData != null && imageData.Length > 0)
            {
                string fileName = mangaId + ".jpg";
                var file = ApplicationData.Current.LocalFolder.CreateFileAsync(Path.Combine(BackgroundImagesFolder, fileName), Windows.Storage.CreationCollisionOption.ReplaceExisting).GetResults();
                
                using (var stream = file.OpenStreamForWriteAsync().Result)
                {
                    stream.Write(imageData, 0, imageData.Length);
                }

                return new BitmapImage(new Uri(fileName));
            }

            return null;
        }

        public async void CreateInitialDb()
        {
            // SQlite database  for manga information
            var dbFile = await this.FileExits(ApplicationData.Current.LocalFolder, "mangapp.db");
            if (dbFile != null)
            {
                await dbFile.DeleteAsync();
            }

            IEnumerable<MangaSummary> mangas;
            using (SQLiteConnection db = new SQLiteConnection("mangapp.db"))
            {
                db.CreateTable<DbMangaListVersion>();
                db.CreateTable<DbMangaSummary>();

                // Populate the manga list from the server information
                Requests requests = new Requests();
                mangas = requests.GetMangaList();

                db.Insert(new DbMangaListVersion(requests.MangaListVersion));
                db.InsertAll(mangas.Select(m => DbMangaSummary.FromMangaSummary(m)));
            }

            // Folders for caching images
            var folder = await this.FolderExists(ApplicationData.Current.LocalFolder, SummaryImagesFolder);
            if (folder != null)
            {
                await folder.DeleteAsync();
            }

            var backgroundFolder = await this.FolderExists(ApplicationData.Current.LocalFolder, BackgroundImagesFolder);
            if (backgroundFolder != null)
            {
                await backgroundFolder.DeleteAsync();
            }

            await ApplicationData.Current.LocalFolder.CreateFolderAsync(SummaryImagesFolder, CreationCollisionOption.ReplaceExisting);
            backgroundFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(BackgroundImagesFolder, CreationCollisionOption.ReplaceExisting);

            // Copy the background images from the installed folder to the app folder
            var installFolder = await Package.Current.InstalledLocation.GetFolderAsync(BackgroundImagesFolder);
            foreach (var file in await installFolder.GetFilesAsync())
            {
                await file.CopyAsync(backgroundFolder, file.Name, NameCollisionOption.ReplaceExisting);
            }

            // Get additional summary and background images from the server
            HttpClient client = new HttpClient();
            foreach (var manga in mangas)
            {
                this.CreateSummaryImage(client, manga);
                this.UpdateBackgroundImage(manga.Id);
            }
        }

        private async void CreateSummaryImage(HttpClient client, MangaSummary manga)
        {
            try
            {
                byte[] imageData = await client.GetByteArrayAsync(manga.SummaryImageUrl);
                if (imageData != null && imageData.Length > 0)
                {
                    string extension = Path.GetExtension(manga.SummaryImageUrl.ToString());
                    string fileName = manga.Id + extension;

                    var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(Path.Combine(SummaryImagesFolder, fileName), CreationCollisionOption.ReplaceExisting);
                    using (var stream = await file.OpenStreamForWriteAsync())
                    {
                        stream.Write(imageData, 0, imageData.Length);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private async Task<StorageFile> FileExits(StorageFolder folder, string fileName)
        {
            try
            {
                return await folder.GetFileAsync(fileName);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        private async Task<StorageFolder> FolderExists(StorageFolder folder, string fileName)
        {
            try
            {
                return await folder.GetFolderAsync(fileName);
            }
            catch (FileNotFoundException)
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

            public string Authors { get; set; }
            public string Artists { get; set; }
            public string Categories { get; set; }

            public int? YearOfRelease { get; set; }
            public int Status { get; set; }
            public int? ReadingDirection { get; set; }

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

                            Authors = string.Join(Database.Separators[0], summary.Authors),
                            Artists = string.Join(Database.Separators[0], summary.Artists),
                            Categories = string.Join(Database.Separators[0], summary.Categories),

                            YearOfRelease = summary.YearOfRelease,
                            Status = (int)summary.Status,
                            ReadingDirection = (int?)summary.ReadingDirection,

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

                            Authors = dbManga.Authors.Split(Database.Separators, StringSplitOptions.RemoveEmptyEntries),
                            Artists = dbManga.Artists.Split(Database.Separators, StringSplitOptions.RemoveEmptyEntries),
                            Categories = dbManga.Categories.Split(Database.Separators, StringSplitOptions.RemoveEmptyEntries),

                            YearOfRelease = dbManga.YearOfRelease,
                            Status = (MangaStatus)dbManga.Status,
                            ReadingDirection = (ReadingDirection?)dbManga.ReadingDirection,

                            LastChapter = dbManga.LastChapter,
                            LastChapterDate = dbManga.LastChapterDate
                        };
            }
        }
    }
}
