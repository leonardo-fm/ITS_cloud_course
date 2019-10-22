using DBManager.Classes;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

namespace DBManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Random r = new Random();

            dbManager dbManager = new dbManager();
            dbManager.userManager.confirmUserToMongoDB("5daed1b5db162f7d5c64fb11");
            
            //for (int i = 0; i < 1000; i++)
            //{
            //    int userIdRandom = r.Next(1, 15);
            //    string photoPath = string.Format("C:/User/photo_{0}", i);

            //    dbManager.photoManager.addPhotoToMongoDB(new photo { userId = userIdRandom, photoPath = photoPath });
                
            //}

            //List<photo> listPhoto = dbInterface.getPhotoOfUser(9);

            //foreach (photo photo in listPhoto)
            //{
            //    Console.WriteLine(photo._id);
            //}

            Console.WriteLine("Comando eseguito");
            Console.ReadLine();
        }      
    }
}
