using MangAppClient.Core.Model;
using MangAppClient.Core.Services;
using System;
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
            CreateInitialDb();
        }

        public void CreateInitialDb()
        {
            mangaList = new ObservableCollection<MangaSummary>
            {
                new MangaSummary("0") { Title = "Manga Title 1", Authors = new string[] { "Author 1"}, Categories = new string[] { "Action"} },
                new MangaSummary("1") { Title = "Manga Title 2", Authors = new string[] { "Author 2"}, Categories = new string[] { "Comedy"} },
                new MangaSummary("2") { Title = "Manga Title 3", Authors = new string[] { "Author 3"}, Categories = new string[] { "Horror"} },
                new MangaSummary("3") { Title = "Manga Title 4", Authors = new string[] { "Author 4"}, Categories = new string[] { "Mystery"} },
                new MangaSummary("4") { Title = "Manga Title 5", Authors = new string[] { "Author 5"}, Categories = new string[] { "One Shot"} },
                new MangaSummary("5") { Title = "Manga Title 6", Authors = new string[] { "Author 6"}, Categories = new string[] { "Action"} },
                new MangaSummary("6") { Title = "Manga Title 7", Authors = new string[] { "Author 7"}, Categories = new string[] { "Comedy"} },
                new MangaSummary("7") { Title = "Manga Title 8", Authors = new string[] { "Author 8"}, Categories = new string[] { "Action"} },
                new MangaSummary("8") { Title = "Manga Title 9", Authors = new string[] { "Author 9"}, Categories = new string[] { "Action"} },
                new MangaSummary("9") { Title = "Manga Title 10", Authors = new string[] { "Author 10"}, Categories = new string[] { "Horror"} },
                new MangaSummary("10") { Title = "Manga Title 11", Authors = new string[] { "Author 11"}, Categories = new string[] { "Comedy"} },
                new MangaSummary("11") { Title = "Manga Title 12", Authors = new string[] { "Author 12"}, Categories = new string[] { "Comedy"} }
            };
        }

        public void AddFavoriteManga(string mangaId)
        {
        }

        public void RemoveFavoriteManga(string mangaId)
        {
        }

        public void UpdateFavoriteManga(string mangaId, int lastChapterRead)
        {
        }

        public string GetDefaultBackgroundImage()
        {
            return new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:/Assets/souleater_bg.jpg"));
        }

        public string GetBackgroundImage(string mangaId)
        {
            return null;
        }

        public string UpdateBackgroundImage(string mangaId)
        {
            throw new System.NotImplementedException();
        }

        public System.Collections.Generic.IEnumerable<Core.Model.MangaSummary> GetMangaList()
        {
            return this.mangaList;
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
