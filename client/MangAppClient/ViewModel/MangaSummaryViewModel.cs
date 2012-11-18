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
            Author = summary.Authors;
            Genre = summary.Categories;
            Artist = summary.Artists;
            Status = summary.Status;
            LastChapter = summary.LastChapter;
            UpdateDate = summary.UpdateDate;
            SummaryImageUrl = summary.SummaryImageUrl;
        }

        public string Name { get; set; }
        public IEnumerable<string> Author { get; set; }
        public IEnumerable<string> Genre { get; set; }
        public IEnumerable<string> Artist { get; set; }
        public MangaStatus Status { get; set; }
        public int LastChapter { get; set; }
        public DateTime UpdateDate { get; set; }
        public Uri SummaryImageUrl { get; set; }

        public MangaGroupViewModel Group { get; set; }
    }
}
