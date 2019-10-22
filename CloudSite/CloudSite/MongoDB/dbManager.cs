using DBManager.Classes;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DBManager.Classes
{
    class dbManager
    {
        private const string HOST = "127.0.0.1";
        private const string PORT = "27017";
        private const string DATABASE_NAME = "progettoCloud";
        private const string DATABASE_ADMINISTRATOR_NAME = "admin";
        private const string DATABASE_ADMINISTRATOR_PASSWORD = "1235";

        private MongoClient databaseConnection = null;
        private IMongoDatabase database;

        public photoManager photoManager { get; set; }
        public userManager userManager { get; set; }

        public dbManager()
        {
            connectionToMongoDB();
            photoManager = new photoManager(database);
            userManager = new userManager(database);
        }
        private void connectionToMongoDB()
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
    }
}
