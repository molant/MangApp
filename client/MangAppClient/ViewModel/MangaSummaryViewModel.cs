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
        public MangaSummaryViewModel(Manga manga)
        {
            Title = manga.Title;
            Description = manga.Description;
            AlternativeNames = manga.AlternativeNames;
            Popularity = manga.Popularity;
            Authors = manga.Authors;
            Categories = manga.Categories;
            Artists = manga.Artists;
            YearOfRelease = manga.YearOfRelease;
            Status = manga.Status;
            ReadingDirection = manga.ReadingDirection;
            SummaryImagePath = new Uri(Path.Combine(ApplicationData.Current.LocalFolder.Path, manga.SummaryImagePath));
            LastChapter = manga.LastChapterUploaded;
            LastChapterDate = manga.LastChapterUploadedDate;
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> AlternativeNames { get; set; }
        public int? Popularity { get; set; }

        public IEnumerable<string> Authors { get; set; }
        public IEnumerable<string> Artists { get; set; }
        public IEnumerable<string> Categories { get; set; }

        public int? YearOfRelease { get; set; }
        public MangaStatus? Status { get; set; }
        public ReadingDirection? ReadingDirection { get; set; }
        public Uri SummaryImagePath { get; set; }

        public int? LastChapter { get; set; }
        public DateTime? LastChapterDate { get; set; }
        public string LastChapterInfo
        {
            get
            {
                if (LastChapterDate.HasValue)
                {
                    return String.Format("Last chapter: {0} - {1}", LastChapter, LastChapterDate.Value.ToString("d"));
                }
                else
                {
                    return "Last chapter: " + LastChapter.ToString();
                }
            }
        }

        public MangaGroupViewModel Group { get; set; }
    }
}
