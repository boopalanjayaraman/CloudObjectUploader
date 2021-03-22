namespace CloudFileUploader
{
    public interface ITokenHandler
    {
        AccessToken Token { get; }

        CloudStorageConfiguration Configuration { get; }

        AccessToken GetToken();

        AccessToken TransformToken(CloudAccessToken token);

    }




}
