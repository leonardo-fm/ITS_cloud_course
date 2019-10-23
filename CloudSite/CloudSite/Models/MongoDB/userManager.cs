using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using CloudSite.Model;

namespace CloudSite.Model
{
    class UserManager
    {
        private IMongoCollection<User> userCollection;
        public UserManager(IMongoDatabase database)
        {
            userCollection = database.GetCollection<User>("users");
        }

        public bool userAlreadyRegistered(User userToCheck)
        {
            try
            {               
                var emailCheck = userCollection.Find(x => x.userEmail == userToCheck.userEmail).ToList();

                if (emailCheck.Count != 0)
                    return true;

                return false;
            }
            catch (Exception)
            {
                throw;
            }
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

                var user = userCollection.Find(x => x._id == _id).ToList();
                User userGet = user[0];

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
            var user = userCollection.Find(x => x._id == _id).ToList();
            if (user.Count == 0)
                return false;

            return true;
        }
    }
}
