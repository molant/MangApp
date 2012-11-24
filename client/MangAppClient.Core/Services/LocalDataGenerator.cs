namespace MangAppClient.Core.Services
{
    using MangAppClient.Core.Model;
    using MangAppClient.Core.Utilities;
    using SQLite;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using Windows.ApplicationModel;
    using Windows.Storage;

    public class LocalDataGenerator
    {
        // Working
        public void CreateInitialDb()
        {
            // Recreate the local files and folders
            var dbFile = FileSystemUtilities.GetFile(ApplicationData.Current.LocalFolder, Constants.DbName);
            if (dbFile != null)
            {
                dbFile.DeleteAsync().AsTask().Wait();
            }

            var summaryFolder = FileSystemUtilities.GetFolder(ApplicationData.Current.LocalFolder, Constants.SummaryImagesFolderPath);
            if (summaryFolder != null)
            {
                summaryFolder.DeleteAsync().AsTask().Wait();
            }

            var backgroundFolder = FileSystemUtilities.GetFolder(ApplicationData.Current.LocalFolder, Constants.BackgroundImagesFolderPath);
            if (backgroundFolder != null)
            {
                backgroundFolder.DeleteAsync().AsTask().Wait();
            }

            var downloadsFolder = FileSystemUtilities.GetFolder(ApplicationData.Current.LocalFolder, Constants.DownloadsFolderPath);
            if (downloadsFolder != null)
            {
                downloadsFolder.DeleteAsync().AsTask().Wait();
            }

            summaryFolder = ApplicationData.Current.LocalFolder.CreateFolderAsync(Constants.SummaryImagesFolderPath, CreationCollisionOption.ReplaceExisting).AsTask().Result;
            backgroundFolder = ApplicationData.Current.LocalFolder.CreateFolderAsync(Constants.BackgroundImagesFolderPath, CreationCollisionOption.ReplaceExisting).AsTask().Result;
            downloadsFolder = ApplicationData.Current.LocalFolder.CreateFolderAsync(Constants.DownloadsFolderPath, CreationCollisionOption.ReplaceExisting).AsTask().Result;

            // Copy the background images from the installed folder to the app folder
            var backgroundInstallFolder = Package.Current.InstalledLocation.GetFolderAsync(Constants.BackgroundImagesFolderPath).AsTask().Result;
            foreach (var file in backgroundInstallFolder.GetFilesAsync().AsTask().Result)
            {
                var copiedFile = file.CopyAsync(backgroundFolder, file.Name, NameCollisionOption.ReplaceExisting).AsTask().Result;
            }

            // Copy the summary images from the installed folder to the app folder
            var summaryInstallFolder = Package.Current.InstalledLocation.GetFolderAsync(Constants.SummaryImagesFolderPath).AsTask().Result;
            foreach (var file in summaryInstallFolder.GetFilesAsync().AsTask().Result)
            {
                var copiedFile = file.CopyAsync(summaryFolder, file.Name, NameCollisionOption.ReplaceExisting).AsTask().Result;
            }

            // Populate the manga list from the server information
            WebData requests = new WebData();
            IEnumerable<Manga> mangas = requests.GetMangaList();

            // Get additional summary and background images from the server
            HttpClient client = new HttpClient();
            foreach (var manga in mangas)
            {
                // TODO: VICENTE
                //this.CreateSummaryImage(client, manga);
                //this.UpdateBackgroundImage(manga);
            }

            // Add mangas to the database
            using (SQLiteConnection db = new SQLiteConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "mangapp.db")))
            {
                db.CreateTable<LocalDataVersion>();
                db.CreateTable<Manga>();

                db.Insert(new LocalDataVersion(requests.MangaListVersion));
                db.InsertAll(mangas);
            }
        }
        
        private void CreateSummaryImage(HttpClient client, Manga manga)
        {
            try
            {
                byte[] imageData = client.GetByteArrayAsync(manga.RemoteSummaryImagePath).Result;
                if (imageData != null && imageData.Length > 0)
                {
                    string fileName = manga.Key + Path.GetExtension(manga.RemoteSummaryImagePath);
                    var file = ApplicationData.Current.LocalFolder.CreateFileAsync(Path.Combine(Constants.SummaryImagesFolderPath, fileName), CreationCollisionOption.ReplaceExisting).AsTask().Result;

                    using (var stream = file.OpenStreamForWriteAsync().Result)
                    {
                        stream.Write(imageData, 0, imageData.Length);
                    }

                    manga.SummaryImagePath = Path.Combine(Constants.SummaryImagesFolderPath, manga.Key + Path.GetExtension(manga.RemoteSummaryImagePath));
                }
            }
            catch (Exception)
            {
                // No image in the server, let's use a random default one
                var folder = FileSystemUtilities.GetFolder(ApplicationData.Current.LocalFolder, Constants.SummaryImagesFolderPath);
                if (folder != null)
                {
                    manga.SummaryImagePath = FileSystemUtilities.GetRandomPath(folder, "default");
                }
                else
                {
                    manga.SummaryImagePath = null;
                }
            }
        }
    }
}
