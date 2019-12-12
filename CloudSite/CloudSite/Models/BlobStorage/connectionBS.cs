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
        public UserBlobStorageManager UserBSManager { get; set; }
        private CloudBlobClient _connection { get; set; }

        public ConnectionBS(string userId)
        {
            ConnectionToBlobStorage();
            UserBSManager = new UserBlobStorageManager(_connection, userId);
        }

        private void ConnectionToBlobStorage()
        {
            string storageConnectionString = "DefaultEndpointsProtocol=https;"
                + "AccountName=" + Variables.ACCOUNT_NAME_FOR_BLOB_STORAGE
                + ";AccountKey=" + Variables.SUBSCRIPTION_KEY_FOR_BLOB_STORAGE
                + ";EndpointSuffix=" + Variables.ENDPOINT_SUFFIX_FOR_BLOB_STORAGE;

            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            _connection = account.CreateCloudBlobClient();
        }
    }
}
