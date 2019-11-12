using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CloudSite.Models.MoongoDB
{
    class PhotoManager
    {
        private IMongoCollection<Photo> photoCollection;

        public PhotoManager(IMongoDatabase database)
        {
            photoCollection = database.GetCollection<Photo>(Variables.NAME_OF_TABLE_FOR_PHOTOS_IN_MONGODB);
        }

        public void AddPhotoToMongoDB(Photo photoToAdd)
        {
            photoCollection.InsertOne(photoToAdd);
        }

        public List<Photo> GetPhotosOfUser(string userId)
        {
            return photoCollection.Find(x => x._userId == userId).ToList();
        }

        public Photo GetPhotoForDetails(string userId, string photoName)
        {
            int indexOfPoint = photoName.IndexOf('.');
            ObjectId photo_id = new ObjectId(photoName.Substring(0, indexOfPoint));
            return photoCollection.Find(x => x._id == photo_id && x._userId == userId).First() as Photo;
        }

        public List<Photo> GetPhotosWithTag(string userId, string tag)
        {
            return photoCollection.Find(x => x.tags.Any(y => y.Contains(tag)) && x._userId == userId).ToList();
        }

        public void RemovePhotos(List<string> photoIdToRemoveWithNoExtension)
        {
            foreach (string photoId in photoIdToRemoveWithNoExtension)
            {
                photoCollection.FindOneAndDelete(x => x._id == new ObjectId(photoId));
            }
        }
    }
}
