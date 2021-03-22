using System;
using System.Collections.Generic;
using System.Text;

namespace CloudFileUploader
{
    public class CloudDocument
    {
        public CloudDocument()
        {

        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string DownloadName { get; set; }

        public byte[] Content { get; set; }

        public string ContentType { get; set; }

    }
}
