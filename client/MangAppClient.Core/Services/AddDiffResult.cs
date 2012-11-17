namespace MangAppClient.Core.Services
{
    using MangAppClient.Core.Model;
    using System.Collections.Generic;

    internal class AddDiffResult : DiffResult
    {
        internal AddDiffResult(int id)
            : base(id)
        { }

        internal string Name { get; set; }

        internal IEnumerable<string> Authors { get; set; }
        
        internal IEnumerable<string> Artists { get; set; }
        
        internal IEnumerable<string> Genres { get; set; }

        internal int LastChapter { get; set; }

        internal MangaStatus Status { get; set; }
    }
}
