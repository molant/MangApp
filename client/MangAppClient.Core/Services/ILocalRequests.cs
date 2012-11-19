namespace MangAppClient.Core.Services
{
    using MangAppClient.Core.Model;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Windows.UI.Xaml.Media.Imaging;

    public interface ILocalRequests
    {
        void CreateInitialDb();

        IEnumerable<Manga> GetMangaList();

        void UpdateMangaList();

        void AddFavoriteManga(Manga manga);

        void RemoveFavoriteManga(Manga manga);

        void UpdateFavoriteManga(Manga manga, int lastChapterRead);

        string GetBackgroundImage(Manga manga);

        string GetDefaultBackgroundImage();

        string UpdateBackgroundImage(Manga manga);
    }
}
