using Microsoft.Azure.Devices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost consoleHost = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => {
        services.AddTransient<Main>();
    })
    .Build();

Main main = consoleHost.Services.GetRequiredService<Main>();
await main.ExecuteAsync(args);

class Main
{
   public async Task<int> ExecuteAsync(string[] args)
    {
        //Parse command line
        if (args.Count() != 1)
            return -1; 
        
        string serviceConnectionString = args[0]; 
        
        ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(serviceConnectionString);
        var fileNotificationReceiver = serviceClient.GetFileNotificationReceiver();
        CancellationToken cancellationToken = new CancellationToken(); 

        while (true) 
        {
            Console.WriteLine("Checking for file upload...");
            FileNotification fileNotification = await fileNotificationReceiver.ReceiveAsync(cancellationToken); 
            if (fileNotification == null)
                continue; 
                
            Console.WriteLine($"Uploaded File: {fileNotification.BlobName}"); 
            await fileNotificationReceiver.CompleteAsync(fileNotification,cancellationToken);
        }
    }
}

