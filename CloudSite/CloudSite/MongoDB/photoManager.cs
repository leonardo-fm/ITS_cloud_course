using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DBManager.Classes
{
    class photoManager
    {
        private IMongoCollection<photo> photoCollection;

        public photoManager(IMongoDatabase database)
        {
            photoCollection = database.GetCollection<photo>("userPhotos");
        }

        public void addPhotoToMongoDB(photo photoToAdd)
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

        public List<photo> getPhotoOfUser(string userId)
        {
            try
            {
                ObjectId _id = new ObjectId(userId);
                return photoCollection.Find(x => x._id == _id).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
