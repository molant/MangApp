using MangAppClient.Core.Model;
using MangAppClient.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MangAppClient.Design
{
    public class MockDatabase : ILocalData
    {
        private IEnumerable<Manga> mangaList;

        public MockDatabase()
        {
            CreateInitialDb();
        }

        public void CreateInitialDb()
        {
            //mangaList = new ObservableCollection<Manga>
            //{
            //    new Manga { Key = "1", Title = "Manga Title 1", AuthorsDb = "Author 1", CategoriesDb = "Action" },
            //    new Manga { Key = "2", Title = "Manga Title 2", AuthorsDb = "Author 2", CategoriesDb = "Comedy" },
            //    new Manga { Key = "3", Title = "Manga Title 3", AuthorsDb = "Author 3", CategoriesDb = "Horror" },
            //    new Manga { Key = "4", Title = "Manga Title 4", AuthorsDb = "Author 4", CategoriesDb = "Mystery" },
            //};
        }

        public void AddFavoriteManga(Manga manga)
        {
        }

        public void RemoveFavoriteManga(Manga manga)
        {
        }

        public void UpdateFavoriteManga(Manga manga, int lastChapterRead)
        {
        }

        public string GetDefaultBackgroundImage()
        {
            return "ms-appx:/Assets/souleater_bg.jpg";
        }

        public string GetBackgroundImage(Manga manga)
        {
            return null;
        }

        public string UpdateBackgroundImage(Manga manga)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Manga> GetMangaList()
        {
            return this.mangaList;
        }

        public IEnumerable<Manga> GetMangaListDesign()
        {
            return this.mangaList;
        }

        public void UpdateMangaList()
        {
            throw new System.NotImplementedException();
        }

        public ObservableCollection<Manga> MangaList
        {
            get { throw new NotImplementedException(); }
        }

        public void UpdateManga(Manga manga)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Manga> GetMangaRecomendations()
        {
            throw new NotImplementedException();
        }

        Task<string> ILocalData.GetBackgroundImage(Manga manga)
        {
            throw new NotImplementedException();
        }


        Task<string> ILocalData.GetDefaultBackgroundImage()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetDefaultSummaryImage()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetSummaryImage(Manga manga)
        {
            throw new NotImplementedException();
        }
    }
}
