using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CloudSite.Model
{
    class PhotoManager
    {
        private IMongoCollection<Photo> photoCollection;

        public PhotoManager(IMongoDatabase database)
        {
            photoCollection = database.GetCollection<Photo>("userPhotos");
        }

        public void addPhotoToMongoDB(Photo photoToAdd)
        {
            try
            {
                photoToAdd._id = ObjectId.GenerateNewId();
                photoCollection.InsertOne(photoToAdd);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Photo> getPhotoOfUser(string userId)
        {
            try
            {
                ObjectId _id = new ObjectId(userId);
                return photoCollection.Find(x => x._userId == _id).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
