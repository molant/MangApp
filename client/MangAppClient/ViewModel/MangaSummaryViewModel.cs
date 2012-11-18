using MangAppClient.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MangAppClient.ViewModel
{
    public class MangaSummaryViewModel : MangAppViewModelBase
    {
        public MangaSummaryViewModel(MangaSummary summary)
        {
            Title = summary.Title;
            Description = summary.Description;
            AlternativeNames = summary.AlternativeNames;
            Popularity = summary.Popularity;
            Authors = summary.Authors;
            Categories = summary.Categories;
            Artists = summary.Artists;
            YearOfRelease = summary.YearOfRelease;
            Status = summary.Status;
            ReadingDirection = summary.ReadingDirection;
            SummaryImagePath = new Uri(Path.Combine(ApplicationData.Current.LocalFolder.Path, summary.SummaryImagePath));
            LastChapter = summary.LastChapter;
            LastChapterDate = summary.LastChapterDate;
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> AlternativeNames { get; set; }
        public int Popularity { get; set; }

        public IEnumerable<string> Authors { get; set; }
        public IEnumerable<string> Artists { get; set; }
        public IEnumerable<string> Categories { get; set; }

        public int? YearOfRelease { get; set; }
        public MangaStatus Status { get; set; }
        public ReadingDirection? ReadingDirection { get; set; }
        public Uri SummaryImagePath { get; set; }

        public int LastChapter { get; set; }
        public DateTime? LastChapterDate { get; set; }

        public MangaGroupViewModel Group { get; set; }
    }
}
