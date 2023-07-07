using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using Azure.Messaging.ServiceBus.Administration;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Azure.Storage.Sas;
using CloudStorageSync.Helper;
using Microsoft.Extensions.Configuration;


namespace CloudStorageSync.Services
{
    public class DataCopyManager
    {
        private readonly IConfiguration? configuration;
        private readonly BlobServiceClient? sourceBlobServiceClient;
        private readonly BlobServiceClient? destinationBlobServiceClient;
        private readonly QueueServiceClient? sourceQueueServiceClient;
        private readonly QueueServiceClient? destinationQueueServiceClient;
        private readonly ShareServiceClient? sourceShareServiceClient;
        private readonly ShareServiceClient? destinationShareServiceClient;
        private readonly TableServiceClient? sourceTableServiceClient;
        private readonly TableServiceClient? destinationTableServiceClient;

        public DataCopyManager(IConfiguration configuration)
        {
            this.configuration = configuration;

            (
                sourceBlobServiceClient,
                destinationBlobServiceClient,
                sourceQueueServiceClient,
                destinationQueueServiceClient,
                sourceShareServiceClient,
                destinationShareServiceClient,
                sourceTableServiceClient,
                destinationTableServiceClient
            ) = StorageClientFactory.CreateClients(configuration);
        }


        public async Task CopyDataAsync()
        {
            await CopyBlobStorageAsync();
            await CopyQueueStorageAsync();
            await CopyFileSharesAsync();
            await CopyTableStorageAsync();
        }

        private async Task CopyBlobStorageAsync()
        {
            if (sourceBlobServiceClient != null && destinationBlobServiceClient != null)
            {
                await foreach (BlobContainerItem containerItem in sourceBlobServiceClient.GetBlobContainersAsync())
                {
                    string sourceContainerName = containerItem.Name;
                    BlobContainerClient sourceBlobContainerClient = sourceBlobServiceClient.GetBlobContainerClient(sourceContainerName);
                    BlobContainerClient destinationBlobContainerClient = destinationBlobServiceClient.GetBlobContainerClient(sourceContainerName);

                    if (!await destinationBlobContainerClient.ExistsAsync())
                    {
                        await destinationBlobContainerClient.CreateAsync();
                    }

                    BlobStorageCopyHelper blobStorageCopyHelper = new BlobStorageCopyHelper(sourceBlobServiceClient, destinationBlobServiceClient);
                    await blobStorageCopyHelper.CopyBlobsAsync(sourceBlobContainerClient, destinationBlobContainerClient);
                }
            }
        }

        private async Task CopyQueueStorageAsync()
        {
            if (sourceQueueServiceClient != null && destinationQueueServiceClient != null)
            {
                QueueStorageCopyHelper queueStorageCopyHelper = new QueueStorageCopyHelper(sourceQueueServiceClient, destinationQueueServiceClient);
                await queueStorageCopyHelper.CopyQueuesAsync();
            }
        }

        private async Task CopyFileSharesAsync()
        {
            if (sourceShareServiceClient != null && destinationShareServiceClient != null)
            {
                FileShareCopyHelper fileShareCopyHelper = new FileShareCopyHelper(sourceShareServiceClient, destinationShareServiceClient);
                await fileShareCopyHelper.CopyFileSharesAsync();
            }
        }

        private async Task CopyTableStorageAsync()
        {
            if (sourceTableServiceClient != null && destinationTableServiceClient != null)
            {
                TableStorageCopyHelper tableStorageCopyHelper = new TableStorageCopyHelper(sourceTableServiceClient, destinationTableServiceClient);
                await tableStorageCopyHelper.CopyTablesAsync();
            }
        }
    }
}
