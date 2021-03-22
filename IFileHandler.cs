namespace CloudFileUploader
{
    public interface IFileHandler
    {
        CloudStorageConfiguration Configuration { get; }

        ITokenHandler TokenHandler { get;  }

        CloudDocument Document { get; }

        void Save(CloudDocument document, int retryCount = 0);

        void Delete(CloudDocument document, int retryCount = 0);

        void Get(CloudDocument document, int retryCount = 0);

    }




}
