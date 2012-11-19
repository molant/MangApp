using MangAppClient.Core.Model;
using MangAppClient.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangAppClient.Design
{
    public class MockService : IWebRequests
    {
        private Manga manga;

        public MockService()
        {
            manga = new Manga() { 
                Title = "Awesome manga", 
                RemoteSummaryImageDb = "ms-appx:/Assets/SOUL_EATER-Portada.jpg",
                ArtistsDb = string.Join("#", new List<string>() { "Awesome author 1", "Awesome author 2", "Awesome author 1" }),
                Description = "You think water moves fast? You should see ice. It moves like it has a mind. Like it knows it killed the world once and got a taste for murder. After the avalanche, it took us a week to climb out. Now, I don't know exactly when we turned on each other, but I know that seven of us survived the slide... and only five made it out. Now we took an oath, that I'm breaking now. We said we'd say it was the snow that killed the other two, but it wasn't. Nature is lethal but it doesn't hold a candle to man. /nYou think water moves fast? You should see ice. It moves like it has a mind. Like it knows it killed the world once and got a taste for murder. After the avalanche, it took us a week to climb out. Now, I don't know exactly when we turned on each other, but I know that seven of us survived the slide... and only five made it out. Now we took an oath, that I'm breaking now. We said we'd say it was the snow that killed the other two, but it wasn't. Nature is lethal but it doesn't hold a candle to man.",
                YearOfRelease = 9999,
                LastChapter = 10,
                StatusDb = 1,
            };

            var chapters = new List<Chapter>();
            for (int i = 1; i <= 50; i++)
                chapters.Add(new Chapter() { Title = "Chapter" + i });

            manga.Chapters = chapters;
        }

        public void GetMangaChapters(Manga manga)
        {
            manga = this.manga;
        }


        public void GetChapterPages(Chapter chapter)
        {
            throw new NotImplementedException();
        }

        public void GetChapterPages(Chapter chapter, int providerKey)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Manga> GetRelatedMangas(Manga manga)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> GetFavoriteMangas(Guid userId)
        {
            throw new NotImplementedException();
        }

        public void DownloadMangaChapter(Chapter chapter)
        {
            throw new NotImplementedException();
        }

        public void DownloadMangaChapters(Chapter start, Chapter end)
        {
            throw new NotImplementedException();
        }
    }
}
