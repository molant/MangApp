namespace MangAppClient.Core.Model
{
    using System;

    public class ChapterSummary
    {
        public int Id { get; set; }
        public DateTime? UploadedDate { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
    }
}
