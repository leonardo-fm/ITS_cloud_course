using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
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

        public ConnectionBS(string userId)
        {
            ConnectionToBlobStorage();
            userBSManager = new UserBlobStorageManager(connection, userId);
        }

        private void ConnectionToBlobStorage()
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
