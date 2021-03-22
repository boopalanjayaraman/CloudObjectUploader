using System;
using System.Collections.Generic;
using System.Text;

namespace CloudFileUploader
{
    public class CloudStorageConfiguration
    {
        public CloudStorageConfiguration()
        {

        }

        public string BaseUrl { get; set; }

        /// <summary>
        /// Also equivalent to ContainerName
        /// </summary>
        public string BucketName { get; set; }

        public string ApiKey { get; set; }

        public string User { get; set; }

        public Cloud Cloud { get; set; }

        public CloudService CloudService { get; set; }

        public string TokenServiceBaseUrl { get; set; }

        public string TokenServiceEndPoint { get; set; }

        public int RetryMaxCount { get; set; }

        public string ConnectionString { get; set; }

    }
}
