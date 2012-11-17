using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangAppClient.Core.Model
{
    public class Chapter
    {
        public string Title { get; set; }
        public IEnumerable<Uri> Pages { get; set; }
    }
}
