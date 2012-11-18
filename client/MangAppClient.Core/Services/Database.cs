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
    using Windows.Storage;
    using Windows.UI.Xaml.Media.Imaging;

    public class Database : IDatabase
    {
        private static readonly string[] Separators = { "#" };

        private static readonly string AppRootPath = ApplicationData.Current.LocalFolder.Path;

        private static readonly string SummaryImagesFolder = "SummaryImages";

        private static readonly string BackgroundImagesFolder = "BackgroundImages";

        public async Task<IEnumerable<MangaSummary>> GetMangaList()
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(Path.Combine(AppRootPath, "mangapp.sqlite"));
            var mangas = await db.Table<DbMangaSummary>().ToListAsync();
            return mangas.Select(m => DbMangaSummary.ToMangaSummary(m));
        }

        public async void UpdateMangaList()
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(Path.Combine(AppRootPath, "mangapp.sqlite"));
            DbMangaListVersion listVersion = await db.Table<DbMangaListVersion>().FirstOrDefaultAsync();

            Requests requests = new Requests();
            var diffs = await requests.GetMangaListDiffAsync(listVersion.Version);

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
                        .Where(m => updates.Any(u => u.Id == m.Id))
                        .ToListAsync();

            var updatedDbMangas = updates.Join(
                dbMangas,
                update => update.Id,
                dbManga => dbManga.Id,
                (update, dbManga) => dbManga.Update(update));

            await db.UpdateAsync(updatedDbMangas);

            // Insert new mangas
            await db.InsertAllAsync(diffs.OfType<MangaSummary>().Select(m => DbMangaSummary.FromMangaSummary(m)));
        }

        public BitmapImage GetDefaultBackgroundImage()
        {
            return new BitmapImage(new Uri(Path.Combine(AppRootPath, "defaultMangaBackground.png")));
        }

        public BitmapImage GetBackgroundImage(int mangaId)
        {
            DbBackgroundImage dbImage;
            using (SQLiteConnection db = new SQLiteConnection(Path.Combine(AppRootPath, "mangapp.sqlite")))
            {
                dbImage = db.Table<DbBackgroundImage>()
                        .Where(b => b.Id == mangaId)
                        .FirstOrDefault();
            }

            if (dbImage == null)
            {
                return null;
            }
            else
            {
                return new BitmapImage(new Uri(dbImage.Path));
            }
        }

        public async Task<BitmapImage> UpdateBackgroundImage(int mangaId)
        {
            byte[] imageData = await new Requests().GetBackgroundImageAsync(mangaId);

            if (imageData != null && imageData.Length > 0)
            {
                string fileName = mangaId + ".png";
                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);
                var stream = await file.OpenStreamForWriteAsync();
                await stream.WriteAsync(imageData, 0, imageData.Length);

                SQLiteAsyncConnection db = new SQLiteAsyncConnection(Path.Combine(AppRootPath, "mangapp.sqlite"));
                await db.InsertAsync(new DbBackgroundImage() { Id = mangaId, Path = fileName });

                return new BitmapImage(new Uri(fileName));
            }

            return null;
        }

        public async void CreateInitialDb()
        {
            await ApplicationData.Current.LocalFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);

            // SQlite database  for manga information
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(Path.Combine(AppRootPath, "mangapp.sqlite"));
            await db.CreateTableAsync<DbMangaListVersion>();
            await db.CreateTableAsync<DbMangaSummary>();
            await db.CreateTableAsync<DbBackgroundImage>();

            // Populate the tables from the server information
            Requests requests = new Requests();
            var mangas = await requests.GetMangaListAsync();

            DbMangaListVersion version = new DbMangaListVersion() { Version = requests.MangaListVersion };
            await db.InsertAsync(version);
            await db.InsertAllAsync(mangas.Select(m => DbMangaSummary.FromMangaSummary(m)));

            // Folders for caching images
            await ApplicationData.Current.LocalFolder.CreateFolderAsync(SummaryImagesFolder, CreationCollisionOption.ReplaceExisting);
            await ApplicationData.Current.LocalFolder.CreateFolderAsync(BackgroundImagesFolder, CreationCollisionOption.ReplaceExisting);

            // Create the local images
            HttpClient client = new HttpClient();

            foreach (var manga in mangas)
            {
                // Summary image
                byte[] imageData = await client.GetByteArrayAsync(manga.SummaryImageUrl);
                if (imageData != null && imageData.Length > 0)
                {
                    string extension = Path.GetExtension(manga.SummaryImageUrl.ToString());
                    string fileName = manga.Id + extension;

                    var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(Path.Combine(AppRootPath, SummaryImagesFolder, fileName), CreationCollisionOption.ReplaceExisting);
                    var stream = await file.OpenStreamForWriteAsync();
                    await stream.WriteAsync(imageData, 0, imageData.Length);
                }

                // Background image
                await this.UpdateBackgroundImage(manga.Id);
            }
        }

        private class DbMangaListVersion
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }

            public int Version { get; set; }
        }

        private class DbMangaSummary
        {
            [PrimaryKey]
            public int Id { get; set; }

            public string Name { get; set; }

            public string Author { get; set; }

            public string Genre { get; set; }

            public string Artist { get; set; }

            public MangaStatus Status { get; set; }

            public int LastChapter { get; set; }

            public DbMangaSummary Update(UpdateDiffResult update)
            {
                this.LastChapter = update.LastChapter;
                if (update.NewStatus.HasValue)
                {
                    this.Status = update.NewStatus.Value;
                }

                return this;
            }

            public static DbMangaSummary FromMangaSummary(MangaSummary summary)
            {
                return new DbMangaSummary()
                {
                    Id = summary.Id,
                    Name = summary.Name,
                    Author = string.Join(Database.Separators[0], summary.Author),
                    Artist = string.Join(Database.Separators[0], summary.Artist),
                    Genre = string.Join(Database.Separators[0], summary.Genre),
                    Status = summary.Status,
                    LastChapter = summary.LastChapter
                };
            }

            public static MangaSummary ToMangaSummary(DbMangaSummary dbManga)
            {
                return new MangaSummary(dbManga.Id)
                        {
                            Name = dbManga.Name,
                            Author = dbManga.Author.Split(Database.Separators, StringSplitOptions.RemoveEmptyEntries),
                            Artist = dbManga.Artist.Split(Database.Separators, StringSplitOptions.RemoveEmptyEntries),
                            Genre = dbManga.Genre.Split(Database.Separators, StringSplitOptions.RemoveEmptyEntries),
                            Status = dbManga.Status,
                            LastChapter = dbManga.LastChapter
                        };
            }
        }

        private class DbBackgroundImage
        {
            [PrimaryKey]
            public int Id { get; set; }

            public string Path { get; set; }
        }
    }
}
