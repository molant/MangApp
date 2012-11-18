namespace MangAppClient.Core.Services
{
    using MangAppClient.Core.Model;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Windows.UI.Xaml.Media.Imaging;

    public interface IDatabase
    {
        void CreateInitialDb();

        IEnumerable<MangaSummary> GetMangaList();

        void UpdateMangaList();

        Uri GetDefaultBackgroundImage();

        string GetBackgroundImage(string mangaId);

        string UpdateBackgroundImage(string mangaId);
    }
}
