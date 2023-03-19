## Azure Environment Creation

- Step 1: Logon to Azure and select the default subscription
- Step 2: Create a project unifier to ensure that IoT Hub and storage account can be created
- Step 3: Create Azure Resource Group 
- Step 4: Create IoT Hub Instance. A S1 SKU of IoT Hub with two partitions will be created. 
- Step 5: Create a storage account and a container where files will be uploaded to. 
- Step 6: Update IoT Hub
    - IoT Hub will be "linked" to created storage account. This allows IoT Hub to create SAS URIs which are used by a device to http post a file to the storage container.

    - Parameter: 
        ```
        //Defines live time of a SaaS created by IoT Hub on request of a device to ne hour (default)
            ...
            $uploadSasTtl = 1  
            ...
            az iot hub update `
                --name $hubName `
                --fileupload-storage-auth-type $storageAuthType `
                --fileupload-storage-connectionstring $storageConnectionString `
                --fileupload-storage-container-name $storageContainerName `
                --fileupload-notifications $uploadNotifications `
                --fileupload-notification-max-delivery-count $uploadNotificationMaxDelivery `
                --fileupload-notification-ttl $uploadNotificationTtl `
                --fileupload-sas-ttl $uploadSasTtl  
        ```
- Step 7: Creates a device, retrieve the DeviceConnection String and start a c# console application simulating a device which uploads 5 files.

- Step 8: Retrieve IoT Hub service connection string and start a c# console application with retrieves file upload notifications from IoT Hub

- Step 9: Housekeeping; used resource group will be deleted.