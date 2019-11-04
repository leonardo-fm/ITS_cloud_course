using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CloudSite.Models.BlobStorage
{
    class UserBlobStorageManager
    {
        private string _userId;
        private CloudBlobClient _connection;
        private CloudBlobContainer _userContainer;

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
            _userContainer = _connection.GetContainerReference(_userId);
            _userContainer.CreateIfNotExistsAsync().Wait();
        }

        public void addPhotoToUserContainer(Stream photo, string photoName)
        {
            if (_userContainer == null)
                throw new ArgumentException("Container is note define", "NullContainer");
            else if (_userContainer.Name != _userId)
                throw new ArgumentException("Container don't mach the user", "NoMatchBetweenContainerAndUser");

            CloudBlockBlob cBlob = _userContainer.GetBlockBlobReference(photoName);
            
            cBlob.UploadFromStream(photo);
            photo.Close();
        } 

        //public void getPhotoByName(string[] photosName)
        public void getPhotoByName()
        {
            var list = _userContainer.ListBlobs("5dc03a8339aa9a2b1093738a.jpg");
        }
    }
}
