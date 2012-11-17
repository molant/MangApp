namespace MangAppClient.Core.Services
{
    internal class Urls
    {
        internal static readonly string BaseUrl = "http://www.mangapp.net:32810";

        internal static string GetMangaList { get { return BaseUrl + "/list"; } }

        internal static string GetMangaDiff { get { return BaseUrl + "/update/{0}"; } }

        internal static string GetMangaDetail { get { return BaseUrl + "/manga/{0}"; } }

        internal static string GetMangaChapter { get { return BaseUrl + "/manga/{0}/{1}"; } }

        internal static string GetMangaChapterFromProvider { get { return BaseUrl + "/manga/{0}/{1}/{2}"; } }

        internal static string GetAuthorMangas { get { return BaseUrl + "/author/{0}"; } }

        internal static string GetRelatedMangas { get { return BaseUrl + "/manga/{0}/related"; } }

        internal static string GetBackgroundImage { get { return BaseUrl + "/manga/{0}/background"; } }

        internal static string GetFavoriteMangas { get { return BaseUrl + "/favorite/{0}"; } }
    }
}
