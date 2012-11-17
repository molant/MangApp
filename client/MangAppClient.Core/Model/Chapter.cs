using System;
using System.Collections.Generic;

namespace MangAppClient.Core.Model
{
    public class Chapter
    {
        public int Id { get; set; }
        public int PreviousChapterId { get; set; }
        public int NextChapterId { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public IEnumerable<Uri> Pages { get; set; }
    }
}
