namespace MangAppClient.Core.Services
{
    internal class Urls
    {
        internal static readonly string GetMangaList = "http://www.mangapp.net/list";

        internal static readonly string GetMangaDiff = "http://www.mangapp.net/update/{0}";

        internal static readonly string GetMangaDetail = "http://www.mangapp.net/manga/{0}";

        internal static readonly string GetMangaChapter = "http://www.mangapp.net/manga/{0}/{1}";

        internal static readonly string GetMangaChapterFromProvider = "http://www.mangapp.net/manga/{0}/{1}/{2}";

        internal static readonly string GetAuthorMangas = "http://www.mangapp.net/author/{0}";

        internal static readonly string GetRelatedMangas = "http://www.mangapp.net/manga/{0}/related";

        internal static readonly string GetBackgroundImage = "http://www.mangapp.net/manga/{0}/background";
    }
}
