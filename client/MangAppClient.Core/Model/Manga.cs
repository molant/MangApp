namespace MangAppClient.Core.Model
{
    using SQLite;
    using System;
    using System.Collections.Generic;

    public class Manga : NotificationObject
    {
        private string key;

        private string providers;

        private string title;

        private string description;

        private string alternativeNames;

        private string authors;

        private string artists;

        private string categories;

        private int? yearOfRelease;

        private int? status;

        private int? readingDirection;

        private int popularity;

        private double averageRating;

        private int lastChapterUploaded;

        private DateTime? lastChapterUploadedDate;

        private string remoteSummaryImagePath;

        private string localSummaryImagePath;

        private int currentChapterReading;

        private int currentPageReading;

        private bool isFavorite;

        private double personalRating;

        [PrimaryKey]
        public string Key
        {
            get { return this.key; }
            internal set { this.SetValue(ref this.key, value); }
        }

        [Ignore]
        public IEnumerable<string> Providers
        {
            get { return this.ProvidersDb.Split(Constants.Separators, StringSplitOptions.RemoveEmptyEntries); }
        }

        internal string ProvidersDb
        {
            get { return this.providers; }
            set
            {
                if (this.SetValue(ref this.providers, value))
                {
                    this.RaisePropertyChanged("Providers");
                }
            }
        }

        public string Title
        {
            get { return this.title; }
            internal set { this.SetValue(ref this.title, value); }
        }

        public string Description
        {
            get { return this.description; }
            internal set { this.SetValue(ref this.description, value); }
        }

        [Ignore]
        public IEnumerable<string> AlternativeNames
        {
            get { return this.AlternativeNamesDb.Split(Constants.Separators, StringSplitOptions.RemoveEmptyEntries); }
        }

        internal string AlternativeNamesDb
        {
            get { return this.alternativeNames; }
            set
            {
                if (this.SetValue(ref this.alternativeNames, value))
                {
                    this.RaisePropertyChanged("AlternativeNames");
                }
            }
        }

        [Ignore]
        public IEnumerable<string> Authors
        {
            get { return this.AuthorsDb.Split(Constants.Separators, StringSplitOptions.RemoveEmptyEntries); }
        }

        internal string AuthorsDb
        {
            get { return this.authors; }
            set
            {
                if (this.SetValue(ref this.authors, value))
                {
                    this.RaisePropertyChanged("Authors");
                }
            }
        }

        [Ignore]
        public IEnumerable<string> Artists
        {
            get { return this.ArtistsDb.Split(Constants.Separators, StringSplitOptions.RemoveEmptyEntries); }
        }

        internal string ArtistsDb
        {
            get { return this.artists; }
            set
            {
                if (this.SetValue(ref this.artists, value))
                {
                    this.RaisePropertyChanged("Artists");
                }
            }
        }

        [Ignore]
        public IEnumerable<string> Categories
        {
            get { return this.CategoriesDb.Split(Constants.Separators, StringSplitOptions.RemoveEmptyEntries); }
        }

        internal string CategoriesDb
        {
            get { return this.categories; }
            set
            {
                if (this.SetValue(ref this.categories, value))
                {
                    this.RaisePropertyChanged("Categories");
                }
            }
        }

        public int? YearOfRelease
        {
            get { return this.yearOfRelease; }
            internal set { this.SetValue(ref this.yearOfRelease, value); }
        }

        [Ignore]
        public MangaStatus? Status
        {
            get
            {
                if (this.StatusDb.HasValue)
                {
                    return (MangaStatus)this.StatusDb.Value;
                }

                return null;
            }
        }

        internal int? StatusDb
        {
            get { return this.status; }
            set
            {
                if (this.SetValue(ref this.status, value))
                {
                    this.RaisePropertyChanged("Status");
                }
            }
        }

        [Ignore]
        public ReadingDirection? ReadingDirection
        {
            get
            {
                if (this.ReadingDirectionDb.HasValue)
                {
                    return (ReadingDirection)this.ReadingDirectionDb.Value;
                }

                return null;
            }
        }

        internal int? ReadingDirectionDb
        {
            get { return this.readingDirection; }
            set
            {
                if (this.SetValue(ref this.readingDirection, value))
                {
                    this.RaisePropertyChanged("ReadingDirection");
                }
            }
        }

        public int Popularity
        {
            get { return this.popularity; }
            internal set { this.SetValue(ref this.popularity, value); }
        }

        public double AverageRating
        {
            get { return this.averageRating; }
            internal set { this.SetValue(ref this.averageRating, value); }
        }

        public int LastChapterUploaded
        {
            get { return this.lastChapterUploaded; }
            internal set { this.SetValue(ref this.lastChapterUploaded, value); }
        }

        public DateTime? LastChapterUploadedDate
        {
            get { return this.lastChapterUploadedDate; }
            internal set { this.SetValue(ref this.lastChapterUploadedDate, value); }
        }

        [Ignore]
        internal string RemoteSummaryImagePath
        {
            get { return this.remoteSummaryImagePath; }
            set { this.SetValue(ref this.remoteSummaryImagePath, value); }
        }

        public string SummaryImagePath
        {
            get { return this.localSummaryImagePath; }
            internal set { this.SetValue(ref this.localSummaryImagePath, value); }
        }

        public int CurrentChapterReading
        {
            get { return this.currentChapterReading; }
            set { this.SetValue(ref this.currentChapterReading, value); }
        }

        public int CurrentPageReading
        {
            get { return this.currentPageReading; }
            set { this.SetValue(ref this.currentPageReading, value); }
        }

        public bool IsFavorite
        {
            get { return this.isFavorite; }
            set { this.SetValue(ref this.isFavorite, value); }
        }

        public double PersonalRating
        {
            get { return this.personalRating; }
            set { this.SetValue(ref this.personalRating, value); }
        }

        [Ignore]
        public IEnumerable<Chapter> Chapters { get; internal set; }

        public Manga()
        {
        }
    }
}
