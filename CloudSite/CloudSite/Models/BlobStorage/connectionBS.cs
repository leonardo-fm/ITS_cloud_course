using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSite.Models.BlobStorage
{
    class ConnectionBS
    {
        public UserBlobStorageManager userBSManager { get; set; }
        private CloudBlobClient connection { get; set; }

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
                + "AccountName=" + Variables.ACCOUNT_NAME_FOR_BLOB_STORAGE
                + ";AccountKey=" + Variables.SUBSCRIPTION_KEY_FOR_BLOB_STORAGE
                + ";EndpointSuffix=" + Variables.ENDPOINT_FOR_BLOB_STORAGE;

            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            connection = account.CreateCloudBlobClient();
        }
    }
}
