using System;
using System.Collections.Generic;
using System.Text;

namespace CloudFileUploader
{
    public class AccessToken
    {
        public AccessToken()
        {

        }

        public string Token { get; set; }

        public string TokenType { get; set; }

        public DateTime ExpiresBy { get; set; }

        public int ExpiresIn { get; set; }

        public string RefreshToken { get; set; }
    }
}
