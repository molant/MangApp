namespace MangAppClient.Core.Services
{
    using MangAppClient.Core.Model;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRequests
    {
        Manga GetMangaDetail(string mangaId);

        Chapter GetChapter(string mangaId, string chapterId);

        Chapter GetChapterFromProvider(string mangaId, string chapterId, int providerId);

        IEnumerable<MangaSummary> GetAuthorMangas(string authorId);

        IEnumerable<MangaSummary> GetRelatedMangas(string mangaId);

        IEnumerable<int> GetFavoriteMangas(int userId);
    }
}
