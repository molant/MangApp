using MangAppClient.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangAppClient.Core.Model
{
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
