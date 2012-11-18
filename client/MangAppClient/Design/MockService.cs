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
            manga = new Manga() { Name = "Awesome manga" };
        }

        public Task<Core.Model.Manga> GetMangaDetailAsync(int mangaId)
        {
            return Task.Factory.StartNew<Manga>(() => { return manga; });
        }

        public Task<Core.Model.Chapter> GetChapterAsync(int mangaId, int chapterId)
        {
            throw new NotImplementedException();
        }

        public Task<Core.Model.Chapter> GetChapterFromProviderAsync(int mangaId, int chapterId, int providerId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Core.Model.MangaSummary>> GetAuthorMangasAsync(string authorId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Core.Model.MangaSummary>> GetRelatedMangasAsync(int mangaId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<int>> GetFavoriteMangasAsync(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
