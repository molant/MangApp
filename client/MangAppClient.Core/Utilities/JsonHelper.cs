namespace MangAppClient.Core.Utilities
{
    using MangAppClient.Core.Model;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class JsonHelper
    {
        public static Manga ParseManga(JToken token)
        {
            // TODO: handle null return in callers to this method!
            int? lastChapter = JsonHelper.ParseInt(token["chapters_len"]);

            if (!lastChapter.HasValue)
            {
                return null;
            }

            Manga manga = new Manga();

            manga.LastChapterUploaded = lastChapter.Value;

            manga.Key = token["_id"].Value<string>();
            manga.Title = token["title"].Value<string>();
            manga.Description = token["description"].Value<string>();
            manga.AlternativeNamesDb = string.Join("#", token["alias"].Children().Values<string>());

            int? popularity = JsonHelper.ParseInt(token["hits"]);
            manga.Popularity = (popularity.HasValue) ? popularity.Value : 0;

            manga.ProvidersDb = string.Join("#", token["providers"].Children().Values<string>());

            manga.AuthorsDb = string.Join("#", token["authors"].Children().Values<string>());
            manga.ArtistsDb = string.Join("#", token["artists"].Children().Values<string>());
            manga.CategoriesDb = string.Join("#", token["categories"].Children().Values<string>());

            manga.YearOfRelease = ParseYear(JsonHelper.ParseInt(token["released"]));
            manga.StatusDb = JsonHelper.ParseInt(token["status"]);
            manga.ReadingDirectionDb = JsonHelper.ParseInt(token["direction"]);

            manga.RemoteSummaryImagePath = token["image"].Value<string>();
            manga.SummaryImagePath = null;

            manga.LastChapterUploadedDate = ParseDateTime(JsonHelper.ParseInt(token["last_chapter_date"]));
            manga.CurrentChapterReading = 1;
            manga.CurrentPageReading = 1;

            JToken chapters = token["chapters"];
            if (chapters != null)
            {
                manga.Chapters = chapters.Children().Select(c => ParseChapter(manga.Key, c)).OrderBy(c => c.Number);
            }

            return manga;
        }

        public static void ParseMangaChapters(Manga manga, JToken token)
        {
            JToken chapters = token["chapters"];
            if (chapters != null)
            {
                manga.Chapters = chapters.Children().Select(c => ParseChapter(manga.Key, c)).OrderBy(c => c.Number);
            }
        }

        public static void ParseChapterPages(Chapter chapter, JToken token)
        {
            JToken pages = token["pages"];
            if (pages != null)
            {
                chapter.Pages = ParsePages(pages.Children());
            }
        }

        private static Chapter ParseChapter(string mangaKey, JToken token)
        {
            Chapter chapter = new Chapter();

            chapter.Key = token["_id"].Value<string>();
            chapter.MangaKey = mangaKey;
            chapter.ProviderKey = JsonHelper.ParseString(token["provider"]);
            chapter.PreviousChapterId = JsonHelper.ParseString(token["previous"]);
            chapter.NextChapterId = JsonHelper.ParseString(token["next"]);
            chapter.Number = JsonHelper.ParseInt(token["number"]);
            chapter.Title = JsonHelper.ParseString(token["title"]);
            chapter.Pages = ParsePages(token["pages"].Children());
            chapter.UploadedDate = ParseDateTime(JsonHelper.ParseInt(token["uploadedDate"]));

            return chapter;
        }

        private static List<string> ParsePages(IEnumerable<JToken> pages)
        {
            return pages
                .Select(t => new { Number = t["number"].Value<int>(), Url = t["url"].Value<string>() })
                .OrderBy(p => p.Number)
                .Select(p => p.Url).ToList();
        }

        private static DateTime? ParseDateTime(int? days)
        {
            if (days.HasValue)
            {
                DateTime dateTime = new DateTime();
                return dateTime.AddDays(days.Value);
            }

            return null;
        }

        private static int? ParseYear(int? days)
        {
            if (days.HasValue)
            {
                DateTime dateTime = new DateTime();
                return dateTime.AddDays(days.Value).Year;
            }

            return null;
        }

        private static string ParseString(JToken stringToken)
        {
            if (stringToken == null)
            {
                return null;
            }

            return stringToken.Value<string>();
        }

        private static int? ParseInt(JToken intToken)
        {
            if (intToken == null)
            {
                return null;
            }

            try
            {
                return intToken.Value<int?>();
            }
            catch
            {
                return null;
            }
        }
    }
}
