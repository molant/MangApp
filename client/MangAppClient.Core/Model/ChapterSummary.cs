namespace MangAppClient.Core.Model
{
    using System;

    public class ChapterSummary
    {
        public string Id { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public DateTime? UploadedDate { get; set; }
    }
}
