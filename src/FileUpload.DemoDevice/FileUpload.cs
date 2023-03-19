using System.Collections.Generic;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport;

internal class FileUpload 
{
    internal DeviceClient _deviceClient; 

    internal readonly int _iotHubDeviceQuota = 10; 

    public FileUpload(DeviceClient deviceClient)
    {
        _deviceClient = deviceClient; 
    }

    internal async Task<bool> IoTHubFileUpload(string fileName, string blobFileName)
    {
        FileUploadCompletionNotification fileUploadCompletionNotification; 

        Console.WriteLine($"Uploading: {fileName} as {blobFileName}");
        using var fileStreamSource = new FileStream(fileName, FileMode.Open);
        fileName = Path.GetFileName(fileStreamSource.Name); 

        var fileUploadSasUriRequest = new FileUploadSasUriRequest {
                BlobName = blobFileName
        };

        FileUploadSasUriResponse sasUri = await _deviceClient.GetFileUploadSasUriAsync(fileUploadSasUriRequest); 
        Uri uploadUri = sasUri.GetBlobUri(); 
        try {
            BlockBlobClient blockBlobClient = new BlockBlobClient(uploadUri);
            await blockBlobClient.UploadAsync(fileStreamSource, new BlobUploadOptions());
        } 
        catch (Exception ex) 
        {
            fileUploadCompletionNotification = new FileUploadCompletionNotification(){
                CorrelationId = sasUri.CorrelationId,
                IsSuccess = false
            };
            await _deviceClient.CompleteFileUploadAsync(fileUploadCompletionNotification);
            Console.WriteLine($"Error during upload: {ex.Message}");
            return false; 
        }

        fileUploadCompletionNotification = new FileUploadCompletionNotification(){
            CorrelationId = sasUri.CorrelationId, 
            IsSuccess = true
        };
        await _deviceClient.CompleteFileUploadAsync(fileUploadCompletionNotification);
        Console.WriteLine($"Successfully uploaded!");

        return true; 
    }
}