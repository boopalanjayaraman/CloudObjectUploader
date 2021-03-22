using RestSharp;
using System;
using System.Net;

namespace CloudFileUploader
{
    /// <summary>
    /// IBM Cloud Object Operations
    /// Need to get a token first using API Key. 
    /// References below:
    /// https://cloud.ibm.com/docs/cloud-object-storage?topic=cloud-object-storage-object-operations
    /// https://cloud.ibm.com/docs/account?topic=account-iamtoken_from_apikey
    /// https://cloud.ibm.com/docs/cloud-object-storage?topic=cloud-object-storage-iam-bucket-permissions
    /// https://cloud.ibm.com/docs/cloud-object-storage?topic=cloud-object-storage-endpoints#endpoints
    /// </summary>
    public class IbmCloudObjectStorageFileHanlder : IFileHandler
    {
        const string DocumentPathFormat =  "{0}/{1}";
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

        public IbmCloudObjectStorageFileHanlder(CloudStorageConfiguration _configuration, ITokenHandler _tokenHandler)
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
            var documentPath = string.Format(DocumentPathFormat, this.Configuration.BucketName, document.Name);
            IRestClient client = new RestClient(this.Configuration.BaseUrl);

            IRestRequest request = new RestRequest(documentPath, Method.DELETE);

            AddAuthorizationHeader(request);
            

            MakeRequest(document, client, request, this.Delete, retryCount);
        }

        private void AddHeaders(IRestRequest request, CloudDocument document)
        {
            if(!string.IsNullOrWhiteSpace(document.ContentType))
            {
                request.AddHeader("Content-Type", document.ContentType);
            }
        }

        /// <summary>
        /// Gets a blob content. Retries the request in case of timeouts / server errors.
        /// </summary>
        /// <param name="document">document object with the name of document</param>
        /// <param name="retryCount">optional - current retry count</param>
        public void Get(CloudDocument document, int retryCount = 0)
        {
            var documentPath = string.Format(DocumentPathFormat, this.Configuration.BucketName, document.Name);
            IRestClient client = new RestClient(this.Configuration.BaseUrl);

            IRestRequest request = new RestRequest(documentPath, Method.GET);

            AddAuthorizationHeader(request);

            MakeRequest(document, client, request, this.Get, retryCount);
        }

        /// <summary>
        /// Uploads a byte content array. Document Name is important and is used to save the blob. 
        /// </summary>
        /// <param name="document"></param>
        /// <param name="retryCount"></param>
        public void Save(CloudDocument document, int retryCount = 0)
        {
            var documentPath = string.Format(DocumentPathFormat, this.Configuration.BucketName, document.Name);
            IRestClient client = new RestClient(this.Configuration.BaseUrl);

            IRestRequest request = new RestRequest(documentPath, Method.PUT);
            request.AddFile(document.Name, document.Content, document.Name);

            AddAuthorizationHeader(request);
            AddHeaders(request, document);

            MakeRequest(document, client, request, this.Save, retryCount);
        }

        private void MakeRequest(CloudDocument input, IRestClient client, IRestRequest request, Action<CloudDocument, int> retryAction, int retryCount)
        {
            IRestResponse<object> response = null;
            if(request.Method == Method.PUT)
            {
                response = client.Put<object>(request);
            }
            else if (request.Method == Method.POST)
            {
                response = client.Post<object>(request);
            }
            else if (request.Method == Method.GET)
            {
                response = client.Get<object>(request);
            }
            else if(request.Method == Method.DELETE)
            {
                response = client.Delete<object>(request);
            }
            else
            {
                throw new Exception("HTTP Method is currently not supported");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.OK
                || response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                retryCount = 0;
                if(request.Method == Method.GET 
                    && response.RawBytes != null)
                {
                    this.Document = input;
                    this.Document.Content = response.RawBytes;
                }
            }
            else if (response.StatusCode == HttpStatusCode.BadGateway
                || response.StatusCode == HttpStatusCode.GatewayTimeout
                || response.StatusCode == HttpStatusCode.InternalServerError
                || response.StatusCode == HttpStatusCode.RequestTimeout
                || response.StatusCode == HttpStatusCode.ServiceUnavailable
                || response.ResponseStatus == ResponseStatus.TimedOut)
            {
                //// retrying logic.
                retryCount++;
                if (retryCount < this.Configuration.RetryMaxCount)
                {
                    //// Log here - retrying attempt {retryCount}
                    retryAction(input, retryCount);
                }
                else
                {
                    //// Log here - retrying attempts failed.
                    throw GetException(response);
                }
            }
            else
            {
                throw GetException(response);
            }
        }

        private static Exception GetException(IRestResponse response)
        {
            var errorMessage = response.ErrorMessage;
            var errorException = response.ErrorException;
            return new Exception(errorMessage, errorException);
        }

        private void AddAuthorizationHeader(IRestRequest request)
        {
            var token = this.TokenHandler.GetToken();
            request.AddHeader("Authorization", String.Format("Bearer {0}", token.Token));
        }
    }




}
