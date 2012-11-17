using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangAppClient.Core.Model
{
    public class Manga
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public string Artist { get; set; }
        public MangaStatus Status { get; set; }
        public int LastChapter { get; set; }
    }
}
