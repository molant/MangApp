namespace MangAppClient.Core.Model
{
    using System;
    using System.Collections.Generic;

    public class Chapter
    {
        public string Id { get; set; }
        public string PreviousChapterId { get; set; }
        public string NextChapterId { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public List<string> Pages { get; set; }
    }
}
