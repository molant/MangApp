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
            WebData web = new WebData();
            IEnumerable<Manga> mangas = web.GetMangaList();

            // HACK: Disabled for testing, if hitting the real server this will produce more than 15k web calls to get images for all the mangas.
            // Get additional summary and background images from the server
            //LocalData local = new LocalData();
            //local.UpdateImageFolderFromServer(backgroundFolder, Constants.DefaultImageName, () => web.GetDefaultBackgroundImages()).Wait();
            //local.UpdateImageFolderFromServer(summaryFolder, Constants.DefaultImageName, () => web.GetDefaultSummaryImages()).Wait();

            //foreach (var manga in mangas)
            //{
            //    local.UpdateImageFolderFromServer(backgroundFolder, manga.Title, () => web.GetBackgroundImages(manga)).Wait();
            //    local.UpdateImageFolderFromServer(summaryFolder, manga.Title, () => web.GetSummaryImages(manga)).Wait();
            //}

            // Add mangas to the database
            using (SQLiteConnection db = new SQLiteConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, Constants.DbName)))
            {
                db.CreateTable<LocalDataVersion>();
                db.CreateTable<Manga>();

                db.Insert(new LocalDataVersion(web.MangaListVersion));
                db.InsertAll(mangas);
            }
        }
    }
}
