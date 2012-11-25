namespace MangAppClient.Core.Model
{
    using SQLite;
    using System;
    using System.Collections.Generic;

    public class Chapter : NotificationObject
    {
        private string key;

        private string mangaKey;

        private string providerKey;

        private int? number;

        private string title;

        private bool isDownloaded;

        private string previousChapterId;

        private string nextChapterId;

        private DateTime? uploadedDate;

        private List<string> pages;

        [PrimaryKey]
        public string Key
        {
            get { return this.key; }
            set { this.SetValue(ref this.key, value); }
        }

        public string MangaKey
        {
            get { return this.mangaKey; }
            set { this.SetValue(ref this.mangaKey, value); }
        }

        public string ProviderKey
        {
            get { return this.providerKey; }
            set { this.SetValue(ref this.providerKey, value); }
        }

        public int? Number
        {
            get { return this.number; }
            set { this.SetValue(ref this.number, value); }
        }

        public string Title
        {
            get { return this.title; }
            set { this.SetValue(ref this.title, value); }
        }

        public bool IsDownloaded
        {
            get { return this.isDownloaded; }
            set { this.SetValue(ref this.isDownloaded, value); }
        }

        public string PreviousChapterId
        {
            get { return this.previousChapterId; }
            set { this.SetValue(ref this.previousChapterId, value); }
        }

        public string NextChapterId
        {
            get { return this.nextChapterId; }
            set { this.SetValue(ref this.nextChapterId, value); }
        }

        public DateTime? UploadedDate
        {
            get { return this.uploadedDate; }
            set { this.SetValue(ref this.uploadedDate, value); }
        }

        [Ignore]
        public List<string> Pages
        {
            get { return this.pages; }
            set { this.SetValue(ref this.pages, value); }
        }

        public Chapter() { }
    }
}
