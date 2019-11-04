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
            photoCollection = database.GetCollection<Photo>("userPhotos");
        }

        public void addPhotoToMongoDB(Photo photoToAdd)
        {
            try
            {
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
                return photoCollection.Find(x => x._userId == userId).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Photo> getPhotoWithTag(string tag)
        {
            try
            {
                var filter = Builders<Photo>.Filter.Eq("tags", new[] { tag });
                return photoCollection.Find(filter).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
