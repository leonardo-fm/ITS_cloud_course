using DBManager.Classes;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DBManager.Classes
{
    class dbInterface
    {
        private const string HOST = "127.0.0.1";
        private const string PORT = "27017";
        private const string DATABASE_NAME = "progettoCloud";
        private const string DATABASE_ADMINISTRATOR_NAME = "admin";
        private const string DATABASE_ADMINISTRATOR_PASSWORD = "1235";
        private const string COLLECTION_NAME = "userPhotos";

        private static MongoClient databaseConnection = null;
        private static IMongoDatabase database;
        private static IMongoCollection<photo> collection;

        public static void connectionToMongoDB()
        {
            try
            {
                string connectionString = string.Format(
                "mongodb://{0}:{1}@{2}:{3}/{4}",
                DATABASE_ADMINISTRATOR_NAME,
                DATABASE_ADMINISTRATOR_PASSWORD,
                HOST,
                PORT,
                DATABASE_NAME
                );

                databaseConnection = new MongoClient(connectionString);
                database = databaseConnection.GetDatabase("progettoCloud");
                collection = database.GetCollection<photo>(COLLECTION_NAME);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void addPhotoToMongoDB(photo userPhoto)
        {
            try
            {
                userPhoto._id = ObjectId.GenerateNewId();
                collection.InsertOne(userPhoto);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<photo> getPhotoOfUser(int userId)
        {
            try
            {
                return collection.Find(x => x.userId == userId).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
