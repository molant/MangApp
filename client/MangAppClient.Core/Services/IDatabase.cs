namespace MangAppClient.Core.Services
{
    using MangAppClient.Core.Model;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Windows.UI.Xaml.Media.Imaging;

    public interface IDatabase
    {
        void CreateInitialDb();

        IEnumerable<MangaSummary> GetMangaList();

        void UpdateMangaList();

        void AddFavoriteManga(string mangaId);

        void RemoveFavoriteManga(string mangaId);

        void UpdateFavoriteManga(string mangaId, int lastChapterRead);

        string GetBackgroundImage(string mangaId);

        string GetDefaultBackgroundImage();

        string UpdateBackgroundImage(string mangaId);
    }
}
