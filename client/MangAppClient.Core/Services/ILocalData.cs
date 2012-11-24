namespace MangAppClient.Core.Services
{
    using MangAppClient.Core.Model;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;

    // TODO: kill this sooner or later
    public interface ILocalData
    {
        ObservableCollection<Manga> MangaList { get; }

        void UpdateMangaList();

        void UpdateManga(Manga manga);

        Task<string> GetDefaultBackgroundImage();

        Task<string> GetBackgroundImage(Manga manga);

        Task<string> GetDefaultSummaryImage();

        Task<string> GetSummaryImage(Manga manga);

        IEnumerable<Manga> GetMangaRecomendations();
    }
}
