using System;
using MongoDB.Driver;

namespace CloudSite.Model
{
    class DBManager
    {
        private const string HOST = "127.0.0.1";
        private const string PORT = "27017";
        private const string DATABASE_NAME = "progettoCloud";
        private const string DATABASE_ADMINISTRATOR_NAME = "admin";
        private const string DATABASE_ADMINISTRATOR_PASSWORD = "1235";

        private MongoClient databaseConnection = null;
        private IMongoDatabase database;

        public PhotoManager photoManager { get; set; }
        public UserManager userManager { get; set; }

        public DBManager()
        {
            connectionToMongoDB();
            photoManager = new PhotoManager(database);
            userManager = new UserManager(database);
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
