using System;
using System.Collections.Generic;

namespace MangAppClient.Core.Model
{
    public class Chapter
    {
        public string Id { get; set; }
        public string PreviousChapterId { get; set; }
        public string NextChapterId { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public IEnumerable<Uri> Pages { get; set; }
    }
}
