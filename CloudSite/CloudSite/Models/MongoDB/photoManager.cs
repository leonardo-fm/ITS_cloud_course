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

        public List<Photo> getPhotoWithTag(string userId, string tag)
        {
            try
            {
                return photoCollection.Find(x => x.tags.Any(y => y.Contains(tag)) && x._userId == userId).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
