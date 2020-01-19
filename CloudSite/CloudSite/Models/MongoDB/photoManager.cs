using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using CloudSite.Models.Photos;

namespace CloudSite.Models.MoongoDB
{
    class PhotoManager
    {
        private IMongoCollection<Photo> _photoCollection;

        public PhotoManager(IMongoDatabase database)
        {
            _photoCollection = database.GetCollection<Photo>(Variables.NAME_OF_TABLE_FOR_PHOTOS_IN_MONGODB);
        }

        public void AddPhotoToMongoDB(Photo photoToAdd)
        {
            _photoCollection.InsertOne(photoToAdd);
        }

        public List<Photo> GetPhotosOfUser(string userId)
        {
            List<Photo> userPhotos = _photoCollection.Find(x => x.UserId == userId).ToList();
            return userPhotos.OrderBy(x => x.PhotoTimeOfUpload).Reverse().ToList();
        }

        public Photo GetPhotoForDetails(string userId, string photoId)
        {
            ObjectId photo_id = new ObjectId(photoId);
            return _photoCollection.Find(x => x._id == photo_id && x.UserId == userId).FirstOrDefault() as Photo;
        }

        public List<Photo> GetPhotosWithTag(string userId, string tag)
        {
            List<Photo> userPhotos = _photoCollection.Find(x => x.Tags.Any(y => y.Contains(tag)) && x.UserId == userId).ToList();
            return userPhotos.OrderBy(x => x.PhotoTimeOfUpload).Reverse().ToList();
        }

        public void RemovePhotos(List<string> photoIdToRemoveWithNoExtension)
        {
            foreach (string photoId in photoIdToRemoveWithNoExtension)
            {
                _photoCollection.FindOneAndDelete(x => x._id == new ObjectId(photoId));
            }
        }
    }
}
