namespace MangAppClient.Core.Model
{
    using SQLite;
    using System;
    using System.Collections.Generic;

    public class Chapter
    {
        [PrimaryKey]
        public string Key { get; set; }
        public string MangaKey { get; set; }
        public string ProviderKey { get; set; }
        public string PreviousChapterId { get; set; }
        public string NextChapterId { get; set; }
        public int? Number { get; set; }
        public string Title { get; set; }
        public DateTime? UploadedDate { get; set; }
        [Ignore]
        public List<string> Pages { get; set; }
    }
}
