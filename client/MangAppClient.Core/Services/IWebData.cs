namespace MangAppClient.Core.Services
{
    using MangAppClient.Core.Model;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IWebData
    {
        void GetMangaChapters(Manga manga);

        void GetChapterPages(Chapter chapter);

        void GetChapterPages(Chapter chapter, int providerKey);

        IEnumerable<Manga> GetRelatedMangas(Manga manga);

        IEnumerable<int> GetFavoriteMangas(Guid userId);

        void DownloadMangaChapter(Chapter chapter);
    }
}
