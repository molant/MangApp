namespace MangAppClient.Core.Services
{
    using MangAppClient.Core.Model;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRequests
    {
        Task<Manga> GetMangaDetailAsync(int mangaId);

        Task<Chapter> GetChapterAsync(int mangaId, int chapterId);

        Task<Chapter> GetChapterFromProviderAsync(int mangaId, int chapterId, int providerId);

        Task<IEnumerable<MangaSummary>> GetAuthorMangasAsync(string authorId);

        Task<IEnumerable<MangaSummary>> GetRelatedMangasAsync(int mangaId);
    }
}
