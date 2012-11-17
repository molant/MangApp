using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangAppClient.Core.Model
{
    public class MangaSummary
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Author { get; set; }
        public IEnumerable<string> Genre { get; set; }
        public IEnumerable<string> Artist { get; set; }
        public MangaStatus Status { get; set; }
        public int LastChapter { get; set; }
    }
}
