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
                Name = "Awesome manga", 
                Image = new Uri("ms-appx:/Assets/SOUL_EATER-Portada.jpg"),
                Artist = new List<string>() { "Awesome author 1", "Awesome author 2", "Awesome author 1" },
                Description = "You think water moves fast? You should see ice. It moves like it has a mind. Like it knows it killed the world once and got a taste for murder. After the avalanche, it took us a week to climb out. Now, I don't know exactly when we turned on each other, but I know that seven of us survived the slide... and only five made it out. Now we took an oath, that I'm breaking now. We said we'd say it was the snow that killed the other two, but it wasn't. Nature is lethal but it doesn't hold a candle to man. /nYou think water moves fast? You should see ice. It moves like it has a mind. Like it knows it killed the world once and got a taste for murder. After the avalanche, it took us a week to climb out. Now, I don't know exactly when we turned on each other, but I know that seven of us survived the slide... and only five made it out. Now we took an oath, that I'm breaking now. We said we'd say it was the snow that killed the other two, but it wasn't. Nature is lethal but it doesn't hold a candle to man.",
                Year = 9999,
                TotalChapters = 10,
                Status = MangaStatus.Ongoing
            };
        }

        public Task<Manga> GetMangaDetailAsync(string mangaId)
        {
            return Task.Factory.StartNew<Manga>(() => { return manga; });
        }

        public Task<Chapter> GetChapterAsync(string mangaId, int chapterId)
        {
            throw new NotImplementedException();
        }

        public Task<Chapter> GetChapterFromProviderAsync(string mangaId, int chapterId, int providerId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MangaSummary>> GetAuthorMangasAsync(string authorId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MangaSummary>> GetRelatedMangasAsync(string mangaId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<int>> GetFavoriteMangasAsync(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
