using Azure.Storage.Files.Shares.Models;
using Azure.Storage.Files.Shares;
using System.Threading.Tasks;

namespace CloudStorageSync.Helper
{
    public class FileShareCopyHelper
    {
        private readonly ShareServiceClient sourceShareServiceClient;
        private readonly ShareServiceClient destinationShareServiceClient;

        public FileShareCopyHelper(ShareServiceClient sourceShareServiceClient, ShareServiceClient destinationShareServiceClient)
        {
            this.sourceShareServiceClient = sourceShareServiceClient;
            this.destinationShareServiceClient = destinationShareServiceClient;
        }

        public async Task CopyFileSharesAsync()
        {
            await foreach (ShareItem shareItem in sourceShareServiceClient.GetSharesAsync())
            {
                var sourceShare = sourceShareServiceClient.GetShareClient(shareItem.Name);
                var destinationShare = destinationShareServiceClient.GetShareClient(shareItem.Name);

                if (!await destinationShare.ExistsAsync())
                {
                    await destinationShare.CreateAsync();
                }
            }
        }
    }
}
