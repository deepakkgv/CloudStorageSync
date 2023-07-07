using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;

namespace CloudStorageSync.Helper
{
    public class BlobStorageCopyHelper
    {
        private readonly BlobServiceClient sourceBlobServiceClient;
        private readonly BlobServiceClient destinationBlobServiceClient;

        public BlobStorageCopyHelper(BlobServiceClient sourceBlobServiceClient, BlobServiceClient destinationBlobServiceClient)
        {
            this.sourceBlobServiceClient = sourceBlobServiceClient;
            this.destinationBlobServiceClient = destinationBlobServiceClient;
        }

        public async Task CopyBlobsAsync(BlobContainerClient sourceContainer, BlobContainerClient destinationContainer)
        {
            await foreach (BlobItem blobItem in sourceContainer.GetBlobsAsync())
            {
                if (blobItem is BlobItem blob)
                {
                    var sourceBlob = sourceContainer.GetBlobClient(blob.Name);
                    var destinationBlob = destinationContainer.GetBlobClient(blob.Name);

                    if (!await destinationBlob.ExistsAsync())
                    {
                        // Download the source blob to memory stream
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            await sourceBlob.DownloadToAsync(memoryStream);

                            // Reset the position of the memory stream to the beginning
                            memoryStream.Position = 0;

                            // Upload the blob from memory stream to the destination container
                            await destinationBlob.UploadAsync(memoryStream);
                        }
                    }
                }
            }
        }
    }
}
