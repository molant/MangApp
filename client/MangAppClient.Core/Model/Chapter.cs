using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangAppClient.Core.Model
{
    public class Chapter
    {
        public int PreviousChapterId { get; set; }
        public int NextChapterId { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public IEnumerable<Uri> Pages { get; set; }
    }
}
