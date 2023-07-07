using Microsoft.Extensions.Configuration;
using CloudStorageSync.Services;

public class Program
{
    public static async Task Main(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        DataCopyManager storageAccountCopy = new DataCopyManager(configuration);
        await storageAccountCopy.CopyDataAsync();
    }
}