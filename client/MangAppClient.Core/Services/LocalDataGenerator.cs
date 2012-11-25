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

    public static class LocalDataGenerator
    {
        // Working
        public static void CreateInitialDb(bool useOnlyLocalData)
        {
            // Recreate the local files and folders
            var dbFile = FileSystemUtilities.GetFile(ApplicationData.Current.LocalFolder, Constants.DbName);
            if (dbFile != null)
            {
                dbFile.DeleteAsync().GetAwaiter().GetResult();
            }

            var summaryFolder = FileSystemUtilities.GetFolder(ApplicationData.Current.LocalFolder, Constants.SummaryImagesFolderPath);
            if (summaryFolder != null)
            {
                summaryFolder.DeleteAsync().GetAwaiter().GetResult();
            }

            var backgroundFolder = FileSystemUtilities.GetFolder(ApplicationData.Current.LocalFolder, Constants.BackgroundImagesFolderPath);
            if (backgroundFolder != null)
            {
                backgroundFolder.DeleteAsync().GetAwaiter().GetResult();
            }

            var downloadsFolder = FileSystemUtilities.GetFolder(ApplicationData.Current.LocalFolder, Constants.DownloadsFolderPath);
            if (downloadsFolder != null)
            {
                downloadsFolder.DeleteAsync().AsTask().GetAwaiter().GetResult();
            }

            summaryFolder = ApplicationData.Current.LocalFolder.CreateFolderAsync(Constants.SummaryImagesFolderPath, CreationCollisionOption.ReplaceExisting).AsTask().Result;
            backgroundFolder = ApplicationData.Current.LocalFolder.CreateFolderAsync(Constants.BackgroundImagesFolderPath, CreationCollisionOption.ReplaceExisting).AsTask().Result;
            downloadsFolder = ApplicationData.Current.LocalFolder.CreateFolderAsync(Constants.DownloadsFolderPath, CreationCollisionOption.ReplaceExisting).AsTask().Result;

            // Copy the background images from the installed folder to the app folder
            var backgroundInstallFolder = FileSystemUtilities.GetFolder(Package.Current.InstalledLocation, Path.Combine(Constants.BinDataFolder, Constants.BackgroundImagesFolderPath));
            if (backgroundFolder != null)
            {
                foreach (var file in backgroundInstallFolder.GetFilesAsync().AsTask().Result)
                {
                    var copiedFile = file.CopyAsync(backgroundFolder, file.Name, NameCollisionOption.ReplaceExisting).AsTask().Result;
                }
            }
            
            // Copy the summary images from the installed folder to the app folder
            var summaryInstallFolder = FileSystemUtilities.GetFolder(Package.Current.InstalledLocation, Path.Combine(Constants.BinDataFolder, Constants.SummaryImagesFolderPath));
            if (summaryFolder != null)
            {
                foreach (var file in summaryInstallFolder.GetFilesAsync().AsTask().Result)
                {
                    var copiedFile = file.CopyAsync(summaryFolder, file.Name, NameCollisionOption.ReplaceExisting).AsTask().Result;
                }
            }

            if (useOnlyLocalData)
            {
                CopyDataFromBinFolder();
            }
            else
            {
                CreateDataFromServerInformation();
            }
        }

        private static void CopyDataFromBinFolder()
        {
            var data = FileSystemUtilities.GetFolder(Package.Current.InstalledLocation, Constants.BinDataFolder);
            foreach (var file in data.GetFilesAsync().AsTask().Result)
            {
                var copiedFile = file.CopyAsync(ApplicationData.Current.LocalFolder, file.Name, NameCollisionOption.ReplaceExisting).AsTask().Result;
            }
        }

        private static void CreateDataFromServerInformation()
        {
            // Populate the manga list from the server information
            WebData web = new WebData();
            IEnumerable<Manga> mangas = web.GetMangas();

            // HACK: Disabled for testing, if hitting the real server this will produce more than 20k web calls to get images for all the mangas.
            // Get additional summary and background images from the server
            //LocalData.UpdateImageFolderFromServer(backgroundFolder, Constants.DefaultImageName, () => web.GetDefaultBackgroundImages()).Wait();
            //LocalData.UpdateImageFolderFromServer(summaryFolder, Constants.DefaultImageName, () => web.GetDefaultSummaryImages()).Wait();

            foreach (var manga in mangas)
            {
                CreateSummaryImageFromRemoteUrl(manga); // TODO: this is not necessary once we have GetSummaryImages working in the server
                //LocalData.UpdateImageFolderFromServer(backgroundFolder, manga.Title, () => web.GetBackgroundImages(manga)).Wait();
                //LocalData.UpdateImageFolderFromServer(summaryFolder, manga.Title, () => web.GetSummaryImages(manga)).Wait();
            }

            // Add mangas to the database
            using (SQLiteConnection db = new SQLiteConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, Constants.DbName)))
            {
                db.CreateTable<LocalDataVersion>();
                db.CreateTable<Manga>();
                db.CreateTable<Provider>();

                db.Insert(new LocalDataVersion(web.MangaListVersion));
                db.InsertAll(mangas);
            }
        }

        // Working
        private static void CreateSummaryImageFromRemoteUrl(Manga manga)
        {
            try
            {
                HttpClient client = new HttpClient();

                byte[] imageData = client.GetByteArrayAsync(manga.RemoteSummaryImagePath).Result;
                if (imageData != null && imageData.Length > 0)
                {
                    string fileName = manga.Title + Path.GetExtension(manga.RemoteSummaryImagePath);

                    var folder = FileSystemUtilities.GetFolder(ApplicationData.Current.LocalFolder, Constants.SummaryImagesFolderPath);
                    var file = folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting).AsTask().Result;

                    using (var stream = file.OpenStreamForWriteAsync().Result)
                    {
                        stream.Write(imageData, 0, imageData.Length);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
