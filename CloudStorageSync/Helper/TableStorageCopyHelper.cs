using Azure.Data.Tables.Models;
using Azure.Data.Tables;
using Azure;

namespace CloudStorageSync.Helper
{
    public class TableStorageCopyHelper
    {
        private readonly TableServiceClient sourceTableServiceClient;
        private readonly TableServiceClient destinationTableServiceClient;

        public TableStorageCopyHelper(TableServiceClient sourceTableServiceClient, TableServiceClient destinationTableServiceClient)
        {
            this.sourceTableServiceClient = sourceTableServiceClient;
            this.destinationTableServiceClient = destinationTableServiceClient;
        }

        public async Task CopyTablesAsync()
        {
            // Query for table names
            List<string> tableNames = new List<string>();
            await foreach (TableItem tableItem in sourceTableServiceClient.QueryAsync())
            {
                tableNames.Add(tableItem.Name);
            }

            // Copy each table to the destination storage account
            foreach (string tableName in tableNames)
            {
                // Get reference to the source table
                TableClient sourceTable = sourceTableServiceClient.GetTableClient(tableName);

                // Get reference to the destination table
                TableClient destinationTable = destinationTableServiceClient.GetTableClient(tableName);

                // Create the destination table if it doesn't exist
                await destinationTable.CreateIfNotExistsAsync();

                // Retrieve entities from the source table
                AsyncPageable<TableEntity> entities = sourceTable.QueryAsync<TableEntity>();

                // Insert entities into the destination table
                await foreach (TableEntity entity in entities)
                {
                    try
                    {
                        // Check if the entity already exists in the destination table
                        TableEntity existingEntity = await destinationTable.GetEntityAsync<TableEntity>(entity.PartitionKey, entity.RowKey);
                        if (existingEntity == null)
                        {
                            // Insert the entity if it doesn't exist
                            await destinationTable.UpsertEntityAsync(entity);
                        }
                    }
                    catch (RequestFailedException ex) when (ex.Status == 404)
                    {
                        // The entity does not exist in the destination table
                        // Insert the entity
                        await destinationTable.UpsertEntityAsync(entity);
                    }
                }
            }
        }
    }
}
