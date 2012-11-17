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
        public IEnumerable<string> Author { get; set; }
        public IEnumerable<string> Genre { get; set; }
        public IEnumerable<string> Artist { get; set; }
        public MangaStatus Status { get; set; }
        public IEnumerable<string> Providers { get; set; }
        public string Description { get; set; }
        public int Year { get; set; }
        public IEnumerable<string> AlternativeNames { get; set; }
        public Uri Image { get; set; }
        public int TotalChapters { get; set; }
        public IEnumerable<Chapter> LastChapters { get; set; }
    }
}
