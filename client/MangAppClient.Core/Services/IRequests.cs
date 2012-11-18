﻿namespace MangAppClient.Core.Services
{
    using MangAppClient.Core.Model;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRequests
    {
        Task<Manga> GetMangaDetailAsync(string mangaId);

        Task<Chapter> GetChapterAsync(string mangaId, int chapterId);

        Task<Chapter> GetChapterFromProviderAsync(string mangaId, int chapterId, int providerId);

        Task<IEnumerable<MangaSummary>> GetAuthorMangasAsync(string authorId);

        Task<IEnumerable<MangaSummary>> GetRelatedMangasAsync(string mangaId);

        Task<IEnumerable<int>> GetFavoriteMangasAsync(int userId);
    }
}
