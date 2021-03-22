using RestSharp;
using System;
using System.Net;

namespace CloudFileUploader
{
    public class IbmTokenHandler : ITokenHandler
    {
        int retryCount = 0;

        public AccessToken Token { get; private set; }

        public CloudStorageConfiguration Configuration { get; private set; }

        public AccessToken GetToken()
        {
            //// Should have at the least 1 minute to be in safer side, and to avoid access denied errors.
            if (this.Token != null && this.Token.ExpiresBy.AddMinutes(-1) > DateTime.Now.ToUniversalTime())
            {
                return this.Token;
            }
            else
            {
                return RequestToken();
            }
        }

        private AccessToken RequestToken()
        {
            //// Get a fresh token, refresh this.Token and return it.
            IRestClient client = new RestClient(this.Configuration.TokenServiceBaseUrl);
            IRestRequest request = new RestRequest(this.Configuration.TokenServiceEndPoint, Method.POST, DataFormat.Json);
            AddHeaders(request);
            AddApiKey(request);

            var response = client.Post<IbmCloudAccessToken>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var accessToken = this.TransformToken(response.Data);
                this.Token = accessToken;
                retryCount = 0;
                return this.Token;
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
                    return RequestToken();
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

        private void AddApiKey(IRestRequest request)
        {
            request.AddParameter("grant_type", "urn:ibm:params:oauth:grant-type:apikey");
            request.AddParameter("apikey", this.Configuration.ApiKey);
        }

        private static void AddHeaders(IRestRequest request)
        {
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
        }

        private static Exception GetException(IRestResponse<IbmCloudAccessToken> response)
        {
            var errorMessage = response.ErrorMessage;
            var errorException = response.ErrorException;
            return new Exception(errorMessage, errorException);
        }

        public AccessToken TransformToken(CloudAccessToken token)
        {
            var ibmToken = (IbmCloudAccessToken)token;
            return new AccessToken()
            {
                Token = ibmToken.access_token,
                ExpiresBy = DateTimeOffset.FromUnixTimeSeconds(ibmToken.expiration).UtcDateTime, //// UtcDateTime
                ExpiresIn = ibmToken.expires_in,
                TokenType = ibmToken.token_type,
                RefreshToken = ibmToken.refresh_token
            };
        }

        public IbmTokenHandler(CloudStorageConfiguration _configuration)
        {
            this.Configuration = _configuration;
        }
    }




}
