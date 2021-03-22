using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudFileUploader
{
    public static class ConfigurationHandler
    {
        public const string ConfigurationSection = "AppSettings";
        public static IConfiguration SetUpConfiguration()
        {
            return new ConfigurationBuilder()
                                            .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                                            .AddEnvironmentVariables()
                                            .Build();
        }
    }
}
