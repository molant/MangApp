using MangAppClient.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangAppClient.ViewModel
{
    public class MangaSummaryViewModel : MangAppViewModelBase
    {
        public MangaSummaryViewModel(MangaSummary summary)
        {
            Name = summary.Title;
            Authors = summary.Authors;
            Categories = summary.Categories;
            Artists = summary.Artists;
            Status = summary.Status;
            LastChapter = summary.LastChapter;
            UpdateDate = summary.UpdateDate;
            SummaryImageUrl = summary.SummaryImageUrl;
        }

        public string Name { get; set; }
        public IEnumerable<string> Authors { get; set; }
        public IEnumerable<string> Categories { get; set; }
        public IEnumerable<string> Artists { get; set; }
        public MangaStatus Status { get; set; }
        public int LastChapter { get; set; }
        public DateTime UpdateDate { get; set; }
        public Uri SummaryImageUrl { get; set; }

        public MangaGroupViewModel Group { get; set; }
    }
}
