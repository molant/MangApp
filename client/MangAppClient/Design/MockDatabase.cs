using MangAppClient.Core.Model;
using MangAppClient.Core.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MangAppClient.Design
{
    public class MockDatabase : IDatabase
    {
        private IEnumerable<Manga> mangaList;

        public MockDatabase()
        {
            CreateInitialDb();
        }

        public void CreateInitialDb()
        {
            mangaList = new ObservableCollection<Manga>
            {
                new Manga { Key = "1", Title = "Manga Title 1", AuthorsDb = "Author 1", CategoriesDb = "Action" },
                new Manga { Key = "2", Title = "Manga Title 2", AuthorsDb = "Author 2", CategoriesDb = "Comedy" },
                new Manga { Key = "3", Title = "Manga Title 3", AuthorsDb = "Author 3", CategoriesDb = "Horror" },
                new Manga { Key = "4", Title = "Manga Title 4", AuthorsDb = "Author 4", CategoriesDb = "Mystery" },
            };
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
            throw new System.NotImplementedException();
        }

        public string GetBackgroundImage(Manga manga)
        {
            throw new System.NotImplementedException();
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
    }
}
