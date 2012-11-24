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

        void DownloadMangaChapter(Chapter chapter);
    }
}
