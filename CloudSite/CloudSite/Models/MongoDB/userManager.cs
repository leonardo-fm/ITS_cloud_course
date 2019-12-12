using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CloudSite.Models.MoongoDB
{
    class UserManager
    {
        private IMongoCollection<User> _userCollection;
        public UserManager(IMongoDatabase database)
        {
            _userCollection = database.GetCollection<User>(Variables.NAME_OF_TABLE_FOR_USERS_IN_MONGODB);
        }

        public void AddUserToMongoDB(User userToAdd)
        {
            userToAdd._id = ObjectId.GenerateNewId();
            _userCollection.InsertOne(userToAdd);
        }

        public bool ConfirmUserToMongoDB(string userId)
        {
            try
            {
                ObjectId _id = new ObjectId(userId);

                User userDB = _userCollection.Find(x => x._id == _id).FirstOrDefault() as User;

                if (userDB == null || userDB.ConfirmedEmail)
                    return false;

                userDB.ConfirmedEmail = true;
                _userCollection.ReplaceOneAsync(x => x._id == _id, userDB);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsTheUserInTheDB(string userId)
        {
            try
            {
                ObjectId _id = new ObjectId(userId);
                var userDB = _userCollection.Find(x => x._id == _id).ToList();
                if (userDB.Count == 0)
                    return false;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsTheEmailInTheDB(string userEmail)
        {
            var userDB = _userCollection.Find(x => x.UserEmail == userEmail).ToList();
            if (userDB.Count == 0)
                return false;

            return true;
        }

        public User GetUserData(string userEmail)
        {
            var userDB = _userCollection.Find(x => x.UserEmail == userEmail).ToList();
            if (userDB.Count == 0)
                return null;

            User user = userDB[0];

            return user;
        }
    }
}
