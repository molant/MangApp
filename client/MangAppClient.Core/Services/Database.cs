namespace MangAppClient.Core.Services
{
    using MangAppClient.Core.Model;
    using SQLite;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Windows.Storage;
    using Windows.Storage.Streams;
    using Windows.UI.Xaml.Media.Imaging;

    public class Database : IDatabase
    {
        private static readonly string[] Separators = { "#" };

        private static readonly string DbRootPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;

        public async void CreateInitialDB()
        {
            using (SQLiteConnection db = new SQLiteConnection(Path.Combine(DbRootPath, "mangapp.sqlite")))
            {
                db.CreateTable<DbMangaListVersion>();
                db.CreateTable<DbManga>();
                db.CreateTable<DbBackgroundImage>();

                // Populate the tables
                Requests requests = new Requests();
                var result = await requests.GetMangaListAsync();

                DbMangaListVersion version = new DbMangaListVersion() { Version = requests.MangaListVersion };
                db.Insert(version);

                foreach (var manga in result)
                {
                    db.Insert(DbManga.FromMangaSummary(manga));
                }
            }
        }

        public BitmapImage GetDefaultBackgroundImage()
        {
            return new BitmapImage(new Uri(Path.Combine(DbRootPath, "defaultMangaBackground.png")));
        }

        public BitmapImage GetBackgroundImage(int mangaId)
        {
            DbBackgroundImage dbImage;
            using (SQLiteConnection db = new SQLiteConnection(Path.Combine(DbRootPath, "mangapp.sqlite")))
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

            string fileName = mangaId + ".png";
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);
            var stream = await file.OpenStreamForWriteAsync();
            await stream.WriteAsync(imageData, 0, imageData.Length);

            SQLiteAsyncConnection db = new SQLiteAsyncConnection(Path.Combine(DbRootPath, "mangapp.sqlite"));
            await db.InsertAsync(new DbBackgroundImage() { Id = mangaId, Path = fileName } );

            return new BitmapImage(new Uri(fileName));
        }

        public async Task<IEnumerable<MangaSummary>> GetMangaList()
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(Path.Combine(DbRootPath, "mangapp.sqlite"));
            var mangas = await db.Table<DbManga>().ToListAsync();
            return mangas.Select(m => DbManga.ToMangaSummary(m));
        }

        public async void UpdateMangaList()
        {
            SQLiteAsyncConnection db = new SQLiteAsyncConnection(Path.Combine(DbRootPath, "mangapp.sqlite"));
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
                await db.DeleteAsync<DbManga>(deletion.Id);
            }

            // Update mangas
            var updates = diffs.OfType<UpdateDiffResult>();
            var dbMangas = await db.Table<DbManga>()
                        .Where(m => updates.Any(u => u.Id == m.Id))
                        .ToListAsync();

            var updatedDbMangas = updates.Join(
                dbMangas, 
                update => update.Id, 
                dbManga => dbManga.Id, 
                (update, dbManga) => dbManga.Update(update));

            await db.UpdateAsync(updatedDbMangas);

            // Insert new mangas
            await db.InsertAllAsync(diffs.OfType<MangaSummary>().Select(m => DbManga.FromMangaSummary(m)));
        }

        private class DbMangaListVersion
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }

            public int Version { get; set; }
        }

        private class DbManga
        {
            [PrimaryKey]
            public int Id { get; set; }

            public string Name { get; set; }

            public string Author { get; set; }

            public string Genre { get; set; }

            public string Artist { get; set; }

            public MangaStatus Status { get; set; }

            public int LastChapter { get; set; }

            public DbManga Update(UpdateDiffResult update)
            {
                this.LastChapter = update.LastChapter;
                if (update.NewStatus.HasValue)
                {
                    this.Status = update.NewStatus.Value;
                }

                return this;
            }

            public static DbManga FromMangaSummary(MangaSummary summary)
            {
                return new DbManga()
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

            public static MangaSummary ToMangaSummary(DbManga dbManga)
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
