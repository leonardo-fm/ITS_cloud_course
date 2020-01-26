using System;
using MongoDB.Driver;

namespace CloudSite.Models.MoongoDB
{
    class DBManager
    {
        public PhotoManager PhotoManager { get; set; }
        public UserManager UserManager { get; set; }

        private MongoClient _databaseConnection = null;
        private IMongoDatabase _database;

        public DBManager()
        {
            ConnectionToMongoDB();
            PhotoManager = new PhotoManager(_database);
            UserManager = new UserManager(_database);
        }
        private void ConnectionToMongoDB()
        {
            try
            {
                string connectionString = string.Format(
                "mongodb://{0}:{1}@{2}:{3}/{4}",
                Variables.USER_FOR_AUTHENTICATION_MONGODB,
                Variables.PASSWORD_FOR_AUTHENTICATION_MONGODB,
                Variables.HOST_FOR_MONGODB,
                Variables.PORT_FOR_MONGODB,
                Variables.ADDITIONAL_PARAMETERS_FOR_CONNECTION
                );

                _databaseConnection = new MongoClient(connectionString);
                _database = _databaseConnection.GetDatabase(Variables.NAME_OF_DATABASE_IN_MONGODB);
            }
            catch (Exception)
            {
                throw new ArgumentException("Connection faild, wrong parameters", "MongoDBNotAbleToConnect");
            }
        }
    }
}
