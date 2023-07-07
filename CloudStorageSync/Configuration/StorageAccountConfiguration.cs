using Microsoft.Extensions.Configuration;

namespace CloudStorageSync
{
    public class StorageAccountConfiguration
    {
        public string? SourceConnectionString { get; }
        public string? DestinationConnectionString { get; }

        public StorageAccountConfiguration(IConfiguration configuration)
        {
            SourceConnectionString = configuration["ConnectionStrings:SourceConnectionString"];
            DestinationConnectionString = configuration["ConnectionStrings:DestinationConnectionString"];
        }
    }
}
