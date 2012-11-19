namespace MangAppClient.Core.Model
{
    using SQLite;
    using System;
    using System.Collections.Generic;

    public class Manga
    {
        private static readonly string[] Separators = { "#" };

        [PrimaryKey]
        public string Key { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [Ignore]
        public IEnumerable<string> AlternativeNames
        {
            get
            {
                return this.AlternativeNamesDb.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
            }
        }
        public int Popularity { get; set; }

        [Ignore]
        public IEnumerable<string> Providers
        {
            get
            {
                return this.ProvidersDb.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        [Ignore]
        public IEnumerable<string> Authors
        {
            get
            {
                return this.AuthorsDb.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
            }
        }
        [Ignore]
        public IEnumerable<string> Artists
        {
            get
            {
                return this.ArtistsDb.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
            }
        }
        [Ignore]
        public IEnumerable<string> Categories
        {
            get
            {
                return this.CategoriesDb.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public int? YearOfRelease { get; set; }
        public MangaStatus Status { get; set; }
        public ReadingDirection? ReadingDirection { get; set; }

        [Ignore]
        public Uri RemoteSummaryImage
        {
            get
            {
                return new Uri(this.RemoteSummaryImageDb);
            }
        }
        public string LocalSummaryImage { get; set; }

        public int LastChapter { get; set; }
        public DateTime? LastChapterDate { get; set; }
        public int? LastChapterRead { get; set; }

        [Ignore]
        public IEnumerable<Chapter> Chapters { get; set; }

        // Properties that are ignored have a property that is used for the DB
        public string AlternativeNamesDb { get; set; }
        public string ProvidersDb { get; set; }
        public string AuthorsDb { get; set; }
        public string ArtistsDb { get; set; }
        public string CategoriesDb { get; set; }
        public string RemoteSummaryImageDb { get; set; }
    }
}
