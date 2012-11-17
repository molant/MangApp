namespace MangAppClient.Core.Services
{
    using MangAppClient.Core.Model;
    using SQLite;
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Windows.Storage;
    using Windows.Storage.Streams;
    using Windows.UI.Xaml.Media.Imaging;

    public class Database
    {
        private static readonly string[] Separators = { "#" };

        private static readonly string DbRootPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;

        public async void CreateInitialDB()
        {
            using (SQLiteConnection db = new SQLiteConnection(Path.Combine(DbRootPath, "mangapp.sqlite")))
            {
                db.CreateTable<DbBackgroundImage>();
                db.CreateTable<DbManga>();

                // Populate the tables
                var result = await Requests.GetMangaListAsync();
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
            byte[] imageData = await Requests.GetBackgroundImageAsync(mangaId);

            string fileName = mangaId + ".png";
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);
            var stream = await file.OpenStreamForWriteAsync();
            await stream.WriteAsync(imageData, 0, imageData.Length);

            SQLiteAsyncConnection db = new SQLiteAsyncConnection(Path.Combine(DbRootPath, "mangapp.sqlite"));
            await db.InsertAsync(new DbBackgroundImage() { Id = mangaId, Path = fileName } );

            return new BitmapImage(new Uri(fileName));
        }

        private class DbBackgroundImage
        {
            [PrimaryKey]
            public int Id { get; set; }

            public string Path { get; set; }
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
    }
}
