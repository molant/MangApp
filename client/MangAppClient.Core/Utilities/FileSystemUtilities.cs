namespace MangAppClient.Core.Utilities
{
    using System;
    using System.IO;
    using System.Linq;
    using Windows.Storage;

    /// <summary>
    /// Utilities to work synchronously with the W8 file system.
    /// </summary>
    public static class FileSystemUtilities
    {
        private static Random random = new Random();

        // Working
        /// <summary>
        /// Gets a file from the file system.
        /// </summary>
        /// <param name="folder">The folder that contains the file.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The file if it exists, null otherwise.</returns>
        public static StorageFile GetFile(StorageFolder folder, string fileName)
        {
            try
            {
                return folder.GetFileAsync(fileName).AsTask().Result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Working
        /// <summary>
        /// Gets a folder from the file system.
        /// </summary>
        /// <param name="folder">The folder that contains the folder.</param>
        /// <param name="folderName">Name of the folder.</param>
        /// <returns>The folder if it exists, null otherwise.</returns>
        public static StorageFolder GetFolder(StorageFolder folder, string folderName)
        {
            try
            {
                return folder.GetFolderAsync(folderName).AsTask().Result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a new filename which starts as the base filename parameter, but has a different end part.
        /// </summary>
        /// <param name="folder">The folder where the name will be checked.</param>
        /// <param name="baseFileName">Base filename to use as root.</param>
        /// <returns>
        /// The new filename.
        /// </returns>
        /// <example>
        /// In case that "naruto" does not exist in folder.
        /// GetNewFileName(folder, "naruto") -&gt; "naruto"
        /// GetNewFileName(folder, "naruto") -&gt; "naruto_1"
        /// GetNewFileName(folder, "naruto") -&gt; "naruto_2"
        /// ...
        ///   </example>
        public static string GetNewFileName(StorageFolder folder, string baseFileName)
        {
            try
            {
                var names = folder.GetFilesAsync()
                                .AsTask().Result
                                .Select(f => Path.GetFileNameWithoutExtension(f.Name));

                string possibleName = baseFileName;
                int index = 1;
                while (true)
                {
                    if (names.Any(n => n.Equals(possibleName, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        possibleName = baseFileName + index++;
                    }
                    else
                    {
                        break;
                    }
                }

                return possibleName;
            }
            catch (Exception)
            {
                return baseFileName;
            }
        }

        /// <summary>
        /// Gets a random path between several that have a similar name.
        /// </summary>
        /// <param name="folder">The folder on which the path will be searched.</param>
        /// <param name="name">The name to search for.</param>
        /// <returns>A random path that is similar to name, or string.Empty if no file is similar.</returns>
        public static string GetRandomPath(StorageFolder folder, string name)
        {
            var possibleFiles = folder.GetFilesAsync().AsTask().Result
                   .Where(f => f.Name.Contains(name))
                   .ToList();

            if (possibleFiles.Count > 0)
            {
                return possibleFiles[random.Next(0, possibleFiles.Count)].Path;
            }

            return string.Empty;
        }
    }
}
