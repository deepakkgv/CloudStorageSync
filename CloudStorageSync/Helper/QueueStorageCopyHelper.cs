using Azure.Storage.Queues.Models;
using Azure.Storage.Queues;
using System.Threading.Tasks;

namespace CloudStorageSync.Helper
{
    public class QueueStorageCopyHelper
    {
        private readonly QueueServiceClient sourceQueueServiceClient;
        private readonly QueueServiceClient destinationQueueServiceClient;

        public QueueStorageCopyHelper(QueueServiceClient sourceQueueServiceClient, QueueServiceClient destinationQueueServiceClient)
        {
            this.sourceQueueServiceClient = sourceQueueServiceClient;
            this.destinationQueueServiceClient = destinationQueueServiceClient;
        }

        public async Task CopyQueuesAsync()
        {
            await foreach (QueueItem queueItem in sourceQueueServiceClient.GetQueuesAsync())
            {
                var destinationQueue = destinationQueueServiceClient.GetQueueClient(queueItem.Name);

                if (!await destinationQueue.ExistsAsync())
                {
                    await destinationQueue.CreateAsync();
                }
            }
        }
    }
}
