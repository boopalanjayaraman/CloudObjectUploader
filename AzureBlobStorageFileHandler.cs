using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Net;

namespace CloudFileUploader
{
    /// <summary>
    /// This implements file operations with Azure Blob Storage Service. Uses SDK rather than directly using REST.
    /// References of REST API below for understanding:
    /// https://docs.microsoft.com/en-us/rest/api/storageservices/blob-service-rest-api
    /// https://docs.microsoft.com/en-us/rest/api/storageservices/authorize-with-shared-key
    /// </summary>
    public class AzureBlobStorageFileHandler : IFileHandler
    {
        public CloudStorageConfiguration Configuration
        {
            get;
            private set;
        }

        public ITokenHandler TokenHandler
        {
            get;
            private set;
        }

        public CloudDocument Document
        {
            get;
            private set;
        }

        public AzureBlobStorageFileHandler(CloudStorageConfiguration _configuration, ITokenHandler _tokenHandler)
        {
            this.Configuration = _configuration;
            this.TokenHandler = _tokenHandler;
        }

        /// <summary>
        /// Deletes a blob content. Retries the request in case of timeouts / server errors.
        /// </summary>
        /// <param name="document">document object with the name of document</param>
        /// <param name="retryCount">optional - current retry count</param>
        public void Delete(CloudDocument document, int retryCount = 0)
        {
            BlobClient blobClient = new BlobClient(this.Configuration.ConnectionString,
                                                                this.Configuration.BucketName,
                                                                document.Name);
            
            Azure.Response response = null;
            try
            {
                response = blobClient.Delete();
            }
            catch (Azure.RequestFailedException ex)
            {
                if (ShouldRetry(ex))
                {
                    retryCount++;
                    if (retryCount < this.Configuration.RetryMaxCount)
                    {
                        Delete(document, retryCount);
                    }
                    else
                    {
                        throw ex;
                    }
                }
                else
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Gets a blob content. Retries the request in case of timeouts / server errors.
        /// </summary>
        /// <param name="document">document object with the name of document</param>
        /// <param name="retryCount">optional - current retry count</param>
        public void Get(CloudDocument document, int retryCount = 0)
        {
            BlobClient blobClient = new BlobClient(this.Configuration.ConnectionString,
                                                    this.Configuration.BucketName,
                                                    document.Name);
            var stream = new MemoryStream();
            

            Azure.Response response = null;
            try
            {
                response = blobClient.DownloadTo(stream);
                //// TODO: See if we can convert the Document property to Stream.
                this.Document = document;
                this.Document.Content = stream.ToArray();
            }
            catch (Azure.RequestFailedException ex)
            {
                if (ShouldRetry(ex))  
                {
                    retryCount++;
                    if (retryCount < this.Configuration.RetryMaxCount)
                    {
                        Get(document, retryCount);
                    }
                    else
                    {
                        throw ex;
                    }
                }
                else
                {
                    throw ex;
                }
            }
        }

        private static bool ShouldRetry(Azure.RequestFailedException ex)
        {
            return ex.Status == (int)HttpStatusCode.BadGateway 
                                || ex.Status == (int)HttpStatusCode.GatewayTimeout 
                                || ex.Status == (int)HttpStatusCode.InternalServerError  
                                || ex.Status == (int)HttpStatusCode.RequestTimeout  
                                || ex.Status == (int)HttpStatusCode.ServiceUnavailable;
        }

        /// <summary>
        /// Uploads a byte content array. Document Name is important and is used to save the blob. 
        /// </summary>
        /// <param name="document"></param>
        /// <param name="retryCount"></param>
        public void Save(CloudDocument document, int retryCount = 0)
        {
            BlobClient blobClient = new BlobClient(this.Configuration.ConnectionString,
                                                    this.Configuration.BucketName,
                                                    document.Name);

            var stream = new MemoryStream(document.Content);
            //stream.Position = 0;

            Azure.Response<BlobContentInfo> response = null;
            try
            {
                response = blobClient.Upload(stream);
            }
            catch (Azure.RequestFailedException ex)
            {
                if (ShouldRetry(ex)) 
                {
                    retryCount++;
                    if (retryCount < this.Configuration.RetryMaxCount)
                    {
                        Save(document, retryCount);
                    }
                    else
                    {
                        throw ex;
                    }
                }
                else
                {
                    throw ex;
                }
            }
        }
    }
}
