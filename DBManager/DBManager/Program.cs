using DBManager.Classes;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Security.Authentication;

/*
 * Pacchetti da installare (MongoDB.Driver, Microsoft.Azure.DocumentDB)
 * 
 * 
 * 
 *     
 */

namespace DBManager
{
    class Program
    {
        private static string host = "127.0.0.1";
        private static string dbName = "photo";
        private static string userName = "adminPhoto";
        private static string password = "adminPhoto";
        private static string collectionName = "userPhoto";

        static void Main(string[] args)
        {
            MongoClientSettings settings = new MongoClientSettings();
            settings.Server = new MongoServerAddress(host, 27017);
            //settings.UseSsl = true;
            settings.SslSettings = new SslSettings();
            settings.SslSettings.EnabledSslProtocols = SslProtocols.Tls12;

            MongoIdentity identity = new MongoInternalIdentity(dbName, userName);
            MongoIdentityEvidence evidence = new PasswordEvidence(password);

            settings.Credential = new MongoCredential("SCRAM-SHA-1", identity, evidence);

            MongoClient client = new MongoClient(settings);

            var database = client.GetDatabase(dbName);
            var todoTaskCollection = database.GetCollection<photo>(collectionName);

            //photo p = new photo(2, @"Somewhere\mia");
            //todoTaskCollection.InsertOne(p);

            var res = todoTaskCollection.Find(x => x.idUser == '1').ToList();
            Console.WriteLine(res);

            Console.WriteLine("Premi invio per continuare...");
            Console.ReadLine();
        }
    }
}
