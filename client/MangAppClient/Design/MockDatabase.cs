using MangAppClient.Core.Model;
using MangAppClient.Core.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MangAppClient.Design
{
    public class MockDatabase : IDatabase
    {
        private IEnumerable<MangaSummary> mangaList;

        public MockDatabase()
        {
            CreateInitialDB();
        }

        public void CreateInitialDB()
        {
            mangaList = new ObservableCollection<MangaSummary>
            {
                new MangaSummary(0) { Name = "Manga Title 1", Author = new string[] { "Author 1"}, Genre = new string[] { "Action"} },
                new MangaSummary(1) { Name = "Manga Title 2", Author = new string[] { "Author 2"}, Genre = new string[] { "Comedy"} },
                new MangaSummary(2) { Name = "Manga Title 3", Author = new string[] { "Author 3"}, Genre = new string[] { "Horror"} },
                new MangaSummary(3) { Name = "Manga Title 4", Author = new string[] { "Author 4"}, Genre = new string[] { "Mystery"} },
                new MangaSummary(4) { Name = "Manga Title 5", Author = new string[] { "Author 5"}, Genre = new string[] { "One Shot"} },
                new MangaSummary(5) { Name = "Manga Title 6", Author = new string[] { "Author 6"}, Genre = new string[] { "Action"} },
                new MangaSummary(6) { Name = "Manga Title 7", Author = new string[] { "Author 7"}, Genre = new string[] { "Comedy"} },
                new MangaSummary(7) { Name = "Manga Title 8", Author = new string[] { "Author 8"}, Genre = new string[] { "Action"} },
                new MangaSummary(8) { Name = "Manga Title 9", Author = new string[] { "Author 9"}, Genre = new string[] { "Action"} },
                new MangaSummary(9) { Name = "Manga Title 10", Author = new string[] { "Author 10"}, Genre = new string[] { "Horror"} },
                new MangaSummary(10) { Name = "Manga Title 11", Author = new string[] { "Author 11"}, Genre = new string[] { "Comedy"} },
                new MangaSummary(11) { Name = "Manga Title 12", Author = new string[] { "Author 12"}, Genre = new string[] { "Comedy"} }
            };
        }

        public Windows.UI.Xaml.Media.Imaging.BitmapImage GetDefaultBackgroundImage()
        {
            throw new System.NotImplementedException();
        }

        public Windows.UI.Xaml.Media.Imaging.BitmapImage GetBackgroundImage(int mangaId)
        {
            throw new System.NotImplementedException();
        }

        public System.Threading.Tasks.Task<Windows.UI.Xaml.Media.Imaging.BitmapImage> UpdateBackgroundImage(int mangaId)
        {
            throw new System.NotImplementedException();
        }

        public System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<Core.Model.MangaSummary>> GetMangaList()
        {
            return Task.Factory.StartNew<IEnumerable<MangaSummary>>(() => { return this.mangaList; });
        }

        public System.Collections.Generic.IEnumerable<Core.Model.MangaSummary> GetMangaListDesign()
        {
            return this.mangaList;
        }

        public void UpdateMangaList()
        {
            throw new System.NotImplementedException();
        }
    }
}
