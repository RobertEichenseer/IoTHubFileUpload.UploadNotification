using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport;
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
    internal static string _fileToUpload = "DemoFile.txt";

    public async Task<int> ExecuteAsync(string[] args)
    {
        //Parse command line
        if (args.Count() != 1)
            return -1; 
        string deviceConnectionString = args[0]; 
        
        //Create file to be uploaded
        File.WriteAllBytes(_fileToUpload, new byte[10000]); 

        //Create default DeviceClient
        using DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Mqtt);
        FileUpload fileUpload = new FileUpload(deviceClient); 

        //Upload file(s)
        Console.WriteLine("File Upload");
        for (int i=0; i<5; i++) {
            string blobFileName = String.Concat(Path.GetFileNameWithoutExtension(_fileToUpload), 
                "_", 
                DateTime.UtcNow.ToString("mmssfff"), 
                Path.GetExtension(_fileToUpload)); 

            await fileUpload.IoTHubFileUpload(_fileToUpload, blobFileName);
        }
        await deviceClient.CloseAsync();
        
        return 0;  
    }
}

