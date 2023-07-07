using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using System;

namespace CloudStorageSync.Helper
{
    public class BlobStorageHelper
    {
        public static async Task AddSharedAccessPolicyAsync(BlobContainerClient containerClient, string policyName)
        {
            var policy = new BlobSasBuilder
            {
                BlobContainerName = containerClient.Name,
                Resource = "c",
                StartsOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTimeOffset.UtcNow.AddDays(7),
            };

            policy.SetPermissions(BlobContainerSasPermissions.Read | BlobContainerSasPermissions.Write);

            var accessPolicy = new BlobSignedIdentifier
            {
                Id = policyName,
                AccessPolicy = new BlobAccessPolicy
                {
                    StartsOn = policy.StartsOn,
                    ExpiresOn = policy.ExpiresOn,
                    Permissions = policy.Permissions.ToString()
                }
            };

            await containerClient.SetAccessPolicyAsync(permissions: new BlobSignedIdentifier[] { accessPolicy });
        }
    }
}
