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

        private static MongoClient databaseConnection = null;
        private static IMongoDatabase database;
        private static IMongoCollection<photo> photoCollection;
        private static IMongoCollection<user> userCollection;

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
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void connectionToMongoDB(string collectionToConnect)
        {
            connectionToMongoDB();

            if (collectionToConnect == "userPhotos")
                photoCollection = database.GetCollection<photo>(collectionToConnect);
            else if (collectionToConnect == "users")
                userCollection = database.GetCollection<user>(collectionToConnect);
            else
                throw new ArgumentException(string.Format("Collection does not exist in the {0} database", DATABASE_NAME), "collectionNotExist");
        }

        public static void addPhotoToMongoDB(photo photoToAdd)
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

        public static void addUserToMongoDB(user userToAdd)
        {
            try
            {
                userToAdd._id = ObjectId.GenerateNewId();
                userCollection.InsertOne(userToAdd);
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
                return photoCollection.Find(x => x.userId == userId).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
