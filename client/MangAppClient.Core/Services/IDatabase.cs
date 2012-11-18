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

        BitmapImage GetDefaultBackgroundImage();

        BitmapImage GetBackgroundImage(string mangaId);

        BitmapImage UpdateBackgroundImage(string mangaId);
    }
}
