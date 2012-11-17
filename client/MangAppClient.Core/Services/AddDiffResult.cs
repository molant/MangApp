namespace MangAppClient.Core.Services
{
    using System.Collections.Generic;

    internal class AddDiffResult : DiffResult
    {
        internal AddDiffResult(int id)
            : base(id)
        { }

        internal string Name { get; set; }

        internal IEnumerable<object> Authors { get; set; }
        
        internal IEnumerable<object> Artists { get; set; }
        
        internal IEnumerable<string> Genres { get; set; }

        internal int LastChapter { get; set; }

        internal MangaStatus Status { get; set; }
    }
}
