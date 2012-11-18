using MangAppClient.Core.Model;
using MangAppClient.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangAppClient.Design
{
    public class MockService : IRequests
    {
        private Manga manga;

        public MockService()
        {
            manga = new Manga() { 
                Title = "Awesome manga", 
                SummaryImageUrl = new Uri("ms-appx:/Assets/SOUL_EATER-Portada.jpg"),
                Artists = new List<string>() { "Awesome author 1", "Awesome author 2", "Awesome author 1" },
                Description = "You think water moves fast? You should see ice. It moves like it has a mind. Like it knows it killed the world once and got a taste for murder. After the avalanche, it took us a week to climb out. Now, I don't know exactly when we turned on each other, but I know that seven of us survived the slide... and only five made it out. Now we took an oath, that I'm breaking now. We said we'd say it was the snow that killed the other two, but it wasn't. Nature is lethal but it doesn't hold a candle to man. /nYou think water moves fast? You should see ice. It moves like it has a mind. Like it knows it killed the world once and got a taste for murder. After the avalanche, it took us a week to climb out. Now, I don't know exactly when we turned on each other, but I know that seven of us survived the slide... and only five made it out. Now we took an oath, that I'm breaking now. We said we'd say it was the snow that killed the other two, but it wasn't. Nature is lethal but it doesn't hold a candle to man.",
                YearOfRelease = 9999,
                LastChapter = 10,
                Status = MangaStatus.Ongoing,
            };

            var chapters = new List<ChapterSummary>();
            for (int i = 1; i <= 12; i++)
                chapters.Add(new ChapterSummary() { Title = "Chapter" + i });

            manga.Chapters = chapters;
        }

        public Manga GetMangaDetail(string mangaId)
        {
            return manga;
        }

        public Chapter GetChapter(string mangaId, int chapterId)
        {
            throw new NotImplementedException();
        }

        public Chapter GetChapterFromProvider(string mangaId, int chapterId, int providerId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MangaSummary> GetAuthorMangas(string authorId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MangaSummary> GetRelatedMangas(string mangaId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> GetFavoriteMangas(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
