using System;

namespace CloudFileUploader
{
    /// <summary>
    /// Class that contains Factory method to get the handler
    /// </summary>
    public static class FileHandlerFactory
    {
        /// <summary>
        /// Gets the file handler based on the configuration
        /// </summary>
        /// <param name="cloudService">cloud enum (comes from configuration)</param>
        /// <param name="configuration">Configuration object</param>
        /// <returns></returns>
        public static IFileHandler GetFileHandler(CloudService cloudService, CloudStorageConfiguration configuration)
        {
            switch(cloudService)
            {
                case CloudService.AzureBlobStorage:
                    return new AzureBlobStorageFileHandler(configuration, null);

                case CloudService.IbmCloudObjectStorage:
                    return new IbmCloudObjectStorageFileHanlder(configuration, new IbmTokenHandler(configuration));

                case CloudService.AzureFileStorage:
                case CloudService.IbmFileStorage:
                    throw new NotImplementedException();

                default:
                    return new AzureBlobStorageFileHandler(configuration, null);
            }
        }
    }
}
