namespace MangAppClient.Core.Services
{
    using MangAppClient.Core.Model;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Windows.UI.Xaml.Media.Imaging;

    public interface IDatabase
    {
        void CreateInitialDb();

        Task<IEnumerable<MangaSummary>> GetMangaList();

        void UpdateMangaList();

        BitmapImage GetDefaultBackgroundImage();

        BitmapImage GetBackgroundImage(int mangaId);

        Task<BitmapImage> UpdateBackgroundImage(int mangaId);
    }
}
