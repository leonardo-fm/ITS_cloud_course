using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnessioneBlobStorage
{
    class ConnectionBS
    {
        private UserBlobStorageManager userBSManager { get; set; }
        public CloudBlobClient connection { get; set; }

        public ConnectionBS() 
        {
            connectionToBlobStorage();
            userBSManager = new UserBlobStorageManager(connection);
        }
        public ConnectionBS(string userId)
        {
            connectionToBlobStorage();
            userBSManager = new UserBlobStorageManager(connection, userId);
        }

        private void connectionToBlobStorage()
        {

            string storageConnectionString = "DefaultEndpointsProtocol=https;"
                + "AccountName=progettocloudstorage"
                + ";AccountKey=Z00ylY9K3AweU3uK4asR+0dVz29dqmqlJjLNa3LnH9eiFClkXGnAaW6OkfZ/Q6brAEtPTpSuSmIX07Le4rrr3g=="
                + ";EndpointSuffix=core.windows.net";

            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            connection = account.CreateCloudBlobClient();

            // Create container. Name must be lower case.
            Console.WriteLine("Creating container...");
            var container = connection.GetContainerReference("mycontainer");
            container.CreateIfNotExistsAsync().Wait();
            
            Console.WriteLine("Creato container");

            // write a blob to the container
            //CloudBlockBlob blob = container.GetBlockBlobReference("helloworld.txt");
            //blob.UploadTextAsync("Hello, World!").Wait();
        }
    }
}
