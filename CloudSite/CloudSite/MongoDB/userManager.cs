using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DBManager.Classes
{
    class userManager
    {
        private IMongoCollection<user> userCollection;
        public userManager(IMongoDatabase database)
        {
            userCollection = database.GetCollection<user>("users");
        }

        public void addUserToMongoDB(user userToAdd)
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

        public void confirmUserToMongoDB(string userId)
        {
            try
            {
                ObjectId _id = new ObjectId(userId);
                var user = userCollection.Find(x => x._id == _id).Limit(1).ToList();
                user userGet = user[0];

                if (!userGet.confirmedEmail)
                {
                    userGet.confirmedEmail = true;
                    userCollection.ReplaceOneAsync(x => x._id == _id, userGet);
                }   
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
