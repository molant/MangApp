namespace MangAppClient.Core.Services
{
    using MangAppClient.Core.Model;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRequests
    {
        Manga GetMangaDetail(Manga manga);

        Chapter GetChapter(Manga manga, Chapter chapter);

        Chapter GetChapterFromProvider(Manga manga, Chapter chapter, int providerKey);

        IEnumerable<Manga> GetRelatedMangas(Manga manga);

        IEnumerable<int> GetFavoriteMangas(int userId);
    }
}
