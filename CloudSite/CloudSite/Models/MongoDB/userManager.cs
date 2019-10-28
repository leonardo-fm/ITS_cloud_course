using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CloudSite.Models.MoongoDB
{
    class UserManager
    {
        private IMongoCollection<User> userCollection;
        public UserManager(IMongoDatabase database)
        {
            userCollection = database.GetCollection<User>("users");
        }

        public void addUserToMongoDB(User userToAdd)
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

        public bool confirmUserToMongoDB(string userId)
        {
            try
            {
                ObjectId _id = new ObjectId(userId);

                var userDB = userCollection.Find(x => x._id == _id).ToList();
                User userGet = userDB[0];

                if (userGet.confirmedEmail)
                    return false;

                userGet.confirmedEmail = true;
                userCollection.ReplaceOneAsync(x => x._id == _id, userGet);
                return true;
            }
            catch (Exception)
            {
                throw new Exception("User not found in the DB");
            }
        }

        public bool isTheUserInTheDB(string userId)
        {
            ObjectId _id = new ObjectId(userId);
            var userDB = userCollection.Find(x => x._id == _id).ToList();
            if (userDB.Count == 0)
                return false;

            return true;
        }

        public bool isTheEmailInTheDB(string userEmail)
        {
            var userDB = userCollection.Find(x => x.userEmail == userEmail).ToList();
            if (userDB.Count == 0)
                return false;

            return true;
        }

        public User getUserData(string userEmail)
        {
            var userDB = userCollection.Find(x => x.userEmail == userEmail).ToList();
            if (userDB.Count == 0)
                return null;

            User user = userDB[0];

            return user;
        }
    }
}
