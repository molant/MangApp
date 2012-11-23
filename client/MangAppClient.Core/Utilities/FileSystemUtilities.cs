namespace MangAppClient.Core.Utilities
{
    using System;
    using Windows.Storage;

    /// <summary>
    /// Utilities to work synchronously with the W8 file system.
    /// </summary>
    public static class FileSystemUtilities
    {
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

    }
}
