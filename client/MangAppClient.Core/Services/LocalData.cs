namespace MangAppClient.Core.Services
{
    using MangAppClient.Core.Model;
    using MangAppClient.Core.Utilities;
    using SQLite;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Windows.ApplicationModel;
    using Windows.Storage;
    using Windows.Storage.Search;
    using Windows.UI.Xaml.Media.Imaging;

    /// <summary>
    /// Class that performs operations with local data or that affect local data.
    /// </summary>
    public class LocalData : ILocalData
    {
        private WebData webData;

        private ObservableCollection<Manga> mangaList;

        public ObservableCollection<Manga> MangaList
        {
            get { return this.mangaList; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalData" /> class.
        /// </summary>
        public LocalData()
        {
            this.webData = new WebData();
        }

        /// <summary>
        /// Initializes the local data.
        /// </summary>
        public void Initialize()
        {
            using (SQLiteConnection db = new SQLiteConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, Constants.DbName)))
            {
                this.mangaList = new ObservableCollection<Manga>(db.Table<Manga>().ToList().OrderByDescending(m => m.Popularity));
            }
        }

        public async void UpdateMangaList()
        {
            //SQLiteAsyncConnection db = new SQLiteAsyncConnection("mangapp.db");
            //DbMangaListVersion listVersion = await db.Table<DbMangaListVersion>().FirstOrDefaultAsync();

            //Requests requests = new Requests();
            //var diffs = requests.GetMangaListDiff(listVersion.Version);

            //// Update our local manga list version
            //listVersion.Version = requests.MangaListVersion;
            //await db.UpdateAsync(listVersion);

            //// Update our local manga list with the diffs from the server
            //// Remove old mangas
            //foreach (var deletion in diffs.OfType<RemoveDiffResult>())
            //{
            //    await db.DeleteAsync<DbMangaSummary>(deletion.Id);
            //}

            //// Update mangas
            //var updates = diffs.OfType<UpdateDiffResult>();
            //var dbMangas = await db.Table<DbMangaSummary>()
            //            .Where(m => updates.Any(u => u.Id == m.Key))
            //            .ToListAsync();

            //var updatedDbMangas = updates.Join(
            //    dbMangas,
            //    update => update.Id,
            //    dbManga => dbManga.Key,
            //    (update, dbManga) => dbManga.Update(update));

            //await db.UpdateAsync(updatedDbMangas);

            //// Insert new mangas
            //await db.InsertAllAsync(diffs.OfType<MangaSummary>().Select(m => DbMangaSummary.FromMangaSummary(m)));
        }

        /// <summary>
        /// Updates a manga information in the database.
        /// </summary>
        /// <param name="manga">The manga to update.</param>
        public void UpdateManga(Manga manga)
        {
            using (SQLiteConnection db = new SQLiteConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, Constants.DbName)))
            {
                db.Update(manga);
            }
        }

        /// <summary>
        /// Gets the path to a default background image.
        /// </summary>
        /// <returns>The path to an image.</returns>
        public async Task<string> GetDefaultBackgroundImage()
        {
            string imagePath = string.Empty;

            var folder = FileSystemUtilities.GetFolder(ApplicationData.Current.LocalFolder, Constants.BackgroundImagesFolderPath);
            if (folder != null)
            {
                imagePath = FileSystemUtilities.GetRandomPath(folder, Constants.DefaultImageName);
            }

            // We do not have the image locally, so we have to go to the server to get it
            if (string.IsNullOrEmpty(imagePath))
            {
                await this.UpdateImageFolderFromServer(folder, Constants.DefaultImageName, () => this.webData.GetDefaultBackgroundImages());
                imagePath = FileSystemUtilities.GetRandomPath(folder, Constants.DefaultImageName);
            }

            return imagePath;
        }

        /// <summary>
        /// Gets the path to a background image of a given manga.
        /// </summary>
        /// <param name="manga">The manga of which we want to obtain a background image.</param>
        /// <returns>The path of an image.</returns>
        public async Task<string> GetBackgroundImage(Manga manga)
        {
            string imagePath = string.Empty;

            var folder = FileSystemUtilities.GetFolder(ApplicationData.Current.LocalFolder, Constants.BackgroundImagesFolderPath);
            if (folder != null)
            {
                imagePath = FileSystemUtilities.GetRandomPath(folder, manga.Key);

                if (string.IsNullOrEmpty(imagePath))
                {
                    imagePath = FileSystemUtilities.GetRandomPath(folder, manga.Title);
                }
            }

            // We do not have the image locally, so we have to go to the server to get it
            if (string.IsNullOrEmpty(imagePath))
            {
                await this.UpdateImageFolderFromServer(folder, manga.Title, () => this.webData.GetBackgroundImages(manga));
                imagePath = FileSystemUtilities.GetRandomPath(folder, manga.Title);
            }

            return imagePath;
        }

        /// <summary>
        /// Gets the path to a default summary image.
        /// </summary>
        /// <returns>The path to an image.</returns>
        public async Task<string> GetDefaultSummaryImage()
        {
            string imagePath = string.Empty;

            var folder = FileSystemUtilities.GetFolder(ApplicationData.Current.LocalFolder, Constants.SummaryImagesFolderPath);
            if (folder != null)
            {
                imagePath = FileSystemUtilities.GetRandomPath(folder, "default");
            }

            // We do not have the image locally, so we have to go to the server to get it
            if (string.IsNullOrEmpty(imagePath))
            {
                await this.UpdateImageFolderFromServer(folder, Constants.DefaultImageName, () => this.webData.GetDefaultSummaryImages());
                imagePath = FileSystemUtilities.GetRandomPath(folder, Constants.DefaultImageName);
            }

            return imagePath;
        }

        /// <summary>
        /// Gets the path to a summary image of a given manga.
        /// </summary>
        /// <param name="manga">The manga of which we want to obtain a background image.</param>
        /// <returns>The path of an image.</returns>
        public async Task<string> GetSummaryImage(Manga manga)
        {
            string imagePath = string.Empty;

            var folder = FileSystemUtilities.GetFolder(ApplicationData.Current.LocalFolder, Constants.SummaryImagesFolderPath);
            if (folder != null)
            {
                imagePath = FileSystemUtilities.GetRandomPath(folder, manga.Key);

                if (string.IsNullOrEmpty(imagePath))
                {
                    imagePath = FileSystemUtilities.GetRandomPath(folder, manga.Title);
                }
            }

            // We do not have the image locally, so we have to go to the server to get it
            if (string.IsNullOrEmpty(imagePath))
            {
                await this.UpdateImageFolderFromServer(folder, manga.Title, () => this.webData.GetSummaryImages(manga));
                imagePath = FileSystemUtilities.GetRandomPath(folder, manga.Title);
            }

            return imagePath;
        }

        public IEnumerable<Manga> GetMangaRecomendations()
        {
            // TODO
            return null;
        }

        internal async Task UpdateImageFolderFromServer(StorageFolder folder, string imagesName, Func<Task<IEnumerable<RemoteImage>>> serverCall)
        {
            var images = await serverCall();
            foreach (var image in images)
            {
                byte[] imageData = image.Data;
                string fileName = FileSystemUtilities.GetNewFileName(folder, imagesName) + Path.GetExtension(image.Name);

                var file = folder.CreateFileAsync(fileName).AsTask().Result;
                using (var stream = file.OpenStreamForWriteAsync().Result)
                {
                    stream.Write(imageData, 0, imageData.Length);
                }
            }
        }
    }
}
