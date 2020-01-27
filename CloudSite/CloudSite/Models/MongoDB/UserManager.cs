using System;
using System.Linq;
using CloudSite.Models.User;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CloudSite.Models.MoongoDB
{
    class UserManager
    {
        private IMongoCollection<UserModel> _userCollection;
        public UserManager(IMongoDatabase database)
        {
            _userCollection = database.GetCollection<UserModel>(Variables.NAME_OF_TABLE_FOR_USERS_IN_MONGODB);
        }

        public void AddUserToMongoDB(UserModel userToAdd)
        {
            userToAdd._id = ObjectId.GenerateNewId();
            _userCollection.InsertOne(userToAdd);
        }

        public bool ConfirmUserToMongoDB(string userId)
        {
            try
            {
                ObjectId _id = new ObjectId(userId);

                UserModel userDB = _userCollection.Find(x => x._id == _id).FirstOrDefault() as UserModel;

                if (userDB == null || userDB.ConfirmedEmail)
                    return false;

                userDB.ConfirmedEmail = true;
                _userCollection.ReplaceOne(x => x._id == _id, userDB);
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
                var userDB = _userCollection.Find(x => x._id == _id).FirstOrDefault();
                if (userDB == null)
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
            var userDB = _userCollection.Find(x => x.UserEmail == userEmail).FirstOrDefault();
            if (userDB == null)
                return false;

            return true;
        }

        public UserModel GetUserData(string userEmail)
        {
            var userDB = _userCollection.Find(x => x.UserEmail == userEmail).FirstOrDefault();
            if (userDB == null)
                return null;

            return userDB;
        }

        public void DeleteUserFromDB(string userId)
        {
            _userCollection.FindOneAndDelete(x => x._id == new ObjectId(userId));
        }
    }
}
