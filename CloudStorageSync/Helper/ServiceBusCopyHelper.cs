using Azure.Messaging.ServiceBus.Administration;

public class ServiceBusCopyHelper
{
    private readonly ServiceBusAdministrationClient sourceServiceBusAdminClient;
    private readonly ServiceBusAdministrationClient destinationServiceBusAdminClient;

    public ServiceBusCopyHelper(ServiceBusAdministrationClient sourceServiceBusAdminClient, ServiceBusAdministrationClient destinationServiceBusAdminClient)
    {
        this.sourceServiceBusAdminClient = sourceServiceBusAdminClient;
        this.destinationServiceBusAdminClient = destinationServiceBusAdminClient;
    }

    public async Task CopyServiceBusAsync()
    {
        if (sourceServiceBusAdminClient != null && destinationServiceBusAdminClient != null)
        {
            await foreach (var sourceTopicProperties in sourceServiceBusAdminClient.GetTopicsAsync())
            {
                var destinationTopicProperties = await destinationServiceBusAdminClient.GetTopicAsync(sourceTopicProperties.Name);

                if (destinationTopicProperties is null)
                {
                    await destinationServiceBusAdminClient.CreateTopicAsync(sourceTopicProperties.Name);
                }

                await foreach (var sourceSubscriptionProperties in sourceServiceBusAdminClient.GetSubscriptionsAsync(sourceTopicProperties.Name))
                {
                    var destinationSubscriptionProperties = await destinationServiceBusAdminClient.GetSubscriptionAsync(sourceTopicProperties.Name, sourceSubscriptionProperties.SubscriptionName);

                    if (destinationSubscriptionProperties is null)
                    {
                        await destinationServiceBusAdminClient.CreateSubscriptionAsync(sourceTopicProperties.Name, sourceSubscriptionProperties.SubscriptionName);
                    }
                }
            }
        }
    }
}
