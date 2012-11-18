namespace MangAppClient.Core.Model
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class MangaSummary : DiffResult
    {
        public MangaSummary(string id)
            : base(id)
        {
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> AlternativeNames { get; set; }
        public int Popularity { get; set; }

        public IEnumerable<string> Authors { get; set; }
        public IEnumerable<string> Artists { get; set; }
        public IEnumerable<string> Categories { get; set; }

        public int? YearOfRelease { get; set; }
        public MangaStatus Status { get; set; }
        public ReadingDirection? ReadingDirection { get; set; }
        internal string SummaryImageUrl { get; set; }
        public string SummaryImagePath { get; set; }

        public int LastChapter { get; set; }
        public DateTime? LastChapterDate { get; set; }
    }
}
