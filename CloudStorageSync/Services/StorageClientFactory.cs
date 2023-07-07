using Azure.Data.Tables;
using Azure.Messaging.ServiceBus.Administration;
using Azure.Storage.Blobs;
using Azure.Storage.Files.Shares;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;


namespace CloudStorageSync.Services
{
    public class StorageClientFactory
    {
        public static (BlobServiceClient, BlobServiceClient, QueueServiceClient, QueueServiceClient, ShareServiceClient, ShareServiceClient, TableServiceClient, TableServiceClient) CreateClients(IConfiguration configuration)
        {
            StorageAccountConfiguration accountConfiguration = new StorageAccountConfiguration(configuration);

            string sourceConnectionString = !string.IsNullOrEmpty(accountConfiguration.SourceConnectionString)
                ? accountConfiguration.SourceConnectionString
                : throw new ArgumentNullException("SourceConnectionString", "Source connection string is required.");

            string destinationConnectionString = !string.IsNullOrEmpty(accountConfiguration.DestinationConnectionString)
                ? accountConfiguration.DestinationConnectionString
                : throw new ArgumentNullException("DestinationConnectionString", "Destination connection string is required.");

            BlobServiceClient sourceBlobServiceClient = new BlobServiceClient(sourceConnectionString);
            BlobServiceClient destinationBlobServiceClient = new BlobServiceClient(destinationConnectionString);
            QueueServiceClient sourceQueueServiceClient = new QueueServiceClient(sourceConnectionString);
            QueueServiceClient destinationQueueServiceClient = new QueueServiceClient(destinationConnectionString);
            ShareServiceClient sourceShareServiceClient = new ShareServiceClient(sourceConnectionString);
            ShareServiceClient destinationShareServiceClient = new ShareServiceClient(destinationConnectionString);
            TableServiceClient sourceTableServiceClient = new TableServiceClient(sourceConnectionString);
            TableServiceClient destinationTableServiceClient = new TableServiceClient(destinationConnectionString);

            return (sourceBlobServiceClient, destinationBlobServiceClient, sourceQueueServiceClient, destinationQueueServiceClient,
                 sourceShareServiceClient, destinationShareServiceClient,
                sourceTableServiceClient, destinationTableServiceClient);
        }
    }
}

