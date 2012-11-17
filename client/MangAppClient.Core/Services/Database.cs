namespace MangAppClient.Core.Services
{
    using MangAppClient.Core.Model;
    using SQLite;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class Database
    {
        private static readonly string[] Separators = { "#" };

        public void CreateDB()
        {
            string dbRootPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
            using (SQLiteConnection db = new SQLiteConnection(Path.Combine(dbRootPath, "mangapp.sqlite")))
            {
                db.CreateTable<DbBackgroundImage>();
                db.CreateTable<DbManga>();
            }
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
