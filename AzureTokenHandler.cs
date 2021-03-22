using System;

namespace CloudFileUploader
{
    /// <summary>
    /// This class is not implemented because it is not necessary. But it is there for future extensibility.
    /// </summary>
    public class AzureTokenHandler : ITokenHandler
    {
        int retryCount = 0;
        public AccessToken Token { get; private set; }

        public CloudStorageConfiguration Configuration { get; private set; }

        public AccessToken GetToken()
        {
            throw new NotImplementedException();
        }

        public AccessToken TransformToken(CloudAccessToken token)
        {
            throw new NotImplementedException();
        }
    }




}
