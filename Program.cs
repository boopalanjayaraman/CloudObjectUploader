using System;
using System.Collections.Generic;
using System.IO;

namespace CloudFileUploader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //// Prepare the configuration
            CloudStorageConfiguration cloudStorageConfiguration = new CloudStorageConfiguration();

            var appConfiguration = ConfigurationHandler.SetUpConfiguration();
            var appSettings = appConfiguration.GetSection(ConfigurationHandler.ConfigurationSection);

            cloudStorageConfiguration.BaseUrl = appSettings["BaseUrl"];
            cloudStorageConfiguration.ApiKey  = appSettings["ApiKey"];
            cloudStorageConfiguration.User = appSettings["User"];
            cloudStorageConfiguration.BucketName = appSettings["BucketName"];
            cloudStorageConfiguration.Cloud = (Cloud)Enum.Parse(typeof(Cloud), appSettings["Cloud"]);
            cloudStorageConfiguration.CloudService = (CloudService)Enum.Parse(typeof(CloudService), appSettings["CloudService"]);
            cloudStorageConfiguration.TokenServiceBaseUrl = appSettings["TokenServiceBaseUrl"];
            cloudStorageConfiguration.TokenServiceEndPoint = appSettings["TokenServiceEndPoint"];
            cloudStorageConfiguration.RetryMaxCount = int.Parse(appSettings["RetryMaxCount"]);
            cloudStorageConfiguration.ConnectionString = appSettings["StorageConnectionString"];

            //// get a file handler instance
            IFileHandler fileHandler = FileHandlerFactory.GetFileHandler(cloudStorageConfiguration.CloudService, cloudStorageConfiguration);

            
            //// Upload a document
            CloudDocument document = new CloudDocument();
            document.Name = "11112.xlsx";
            document.Content = File.ReadAllBytes("Doc - v9.xlsx");
            fileHandler.Save(document);
            Console.WriteLine("Upload - Done");
            

            
            //// Download the document
            CloudDocument document1 = new CloudDocument();
            document1.Name = "11112.xlsx";
            fileHandler.Get(document1);
            File.WriteAllBytes(document1.Name, fileHandler.Document.Content);
            Console.WriteLine("Download - Done");



            //// Delete the document
            CloudDocument document2 = new CloudDocument();
            document2.Name = "11112.xlsx";
            fileHandler.Delete(document2);
            Console.WriteLine("Delete - Done");
            Console.WriteLine("Done");

            return;
        }
    }
}
