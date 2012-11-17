namespace MangAppClient.Core.Model
{
    using System.Collections.Generic;

    public class MangaSummary : DiffResult
    {
        public MangaSummary(int id)
            : base(id)
        {
        }

        public string Name { get; set; }
        public IEnumerable<string> Author { get; set; }
        public IEnumerable<string> Genre { get; set; }
        public IEnumerable<string> Artist { get; set; }
        public MangaStatus Status { get; set; }
        public int LastChapter { get; set; }
    }
}
