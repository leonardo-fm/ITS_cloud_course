using Microsoft.Azure.Storage.Blob;
using System;
using System.IO;

namespace CloudSite.Models.BlobStorage
{
    class UserBlobStorageManager
    {
        private string _userId;
        private CloudBlobClient _connection;

        public CloudBlobContainer userContainer;

        public UserBlobStorageManager(CloudBlobClient connection)
        {
            _connection = connection;
        }
        public UserBlobStorageManager(CloudBlobClient connection, string userId)
        {
            _connection = connection;
            _userId = userId;

            selectConteinerUser();
        }

        public void changeUserSelected(string userId)
        {
            _userId = userId;
            selectConteinerUser();
        }

        private void selectConteinerUser()
        {
            userContainer = _connection.GetContainerReference(_userId);
            userContainer.CreateIfNotExistsAsync().Wait();
        }

        public void addPhotoToUserContainer(Stream photo, string photoName)
        {
            if (userContainer == null)
                throw new ArgumentException("Container is note define", "NullContainer");
            else if (userContainer.Name != _userId)
                throw new ArgumentException("Container don't mach the user", "NoMatchBetweenContainerAndUser");

            CloudBlockBlob cBlob = userContainer.GetBlockBlobReference(photoName);
            
            cBlob.UploadFromStream(photo);
            photo.Close();
        }

        //https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-service-sas-create-dotnet
        public string GetContainerSasUri(CloudBlobContainer container)
        {
            string sasContainerToken;
            int timeDifferencesInMinutes = (DateTime.Now.Hour - DateTime.UtcNow.Hour) * 60;

            SharedAccessBlobPolicy adHocPolicy = new SharedAccessBlobPolicy()
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(1 + timeDifferencesInMinutes),
                Permissions = SharedAccessBlobPermissions.Read
            };

            sasContainerToken = container.GetSharedAccessSignature(adHocPolicy, null);
            
            return sasContainerToken;
        }
    }
}
