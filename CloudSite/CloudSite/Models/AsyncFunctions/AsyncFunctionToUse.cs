using CloudSite.Models.BlobStorage;
using CloudSite.Models.ComputerVision;
using CloudSite.Models.ConvalidationUserAuth;
using CloudSite.Models.EmailSender;
using CloudSite.Models.MoongoDB;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CloudSite.Models.AsyncFunctions
{
    public class AsyncFunctionToUse
    {
        public static void sendMailForConvalidation(User user)
        {
            Task t1 = new Task(() =>
            {
                DBManager dbm = new DBManager();
                ConvalidationUser cu = new ConvalidationUser(user);

                user.userPassword = cu.cryptUserPassword(user.userPassword);

                dbm.userManager.addUserToMongoDB(user);

                DefaultBodyText df = new DefaultBodyText(user.userName, user._id);
                string text = df.getNewBodyForEmailSubscription();
                SendMail sm = new SendMail();
                sm.sendNewEmail(user.userEmail, text);
            });

            t1.Start();
        }

        #region UploadPhoto
        public static void uploadPhoto(HttpPostedFileBase file, string userId)
        {
            Photo userPhoto = new Photo();
            userPhoto.imageName = file.FileName;

            string extension = file.FileName.Substring(file.FileName.IndexOf('.'));

            Stream photoForCV = new MemoryStream();
            Stream photoForBS = new MemoryStream();

            using (Stream photo = file.InputStream)
            {
                photo.CopyTo(photoForCV);

                photoForCV.Seek(0, SeekOrigin.Begin);
                photo.Seek(0, SeekOrigin.Begin);

                photo.CopyTo(photoForBS);
                photoForBS.Seek(0, SeekOrigin.Begin);
                photo.Seek(0, SeekOrigin.Begin);
            }

            Task t1 = new Task(() => sendImageToCVandBS(userPhoto, photoForCV, photoForBS, userId, extension));
            t1.Start();
        }

        private static void sendImageToCVandBS(Photo userPhoto, Stream photoForCV, Stream photoForBS, string userId, string extension)
        {

            Task<string[]> t1 = new Task<string[]>(() => ComputerVisionConnection.uploadImageAndHandleTagsResoult(photoForCV));
            Task t2 = new Task(() => uploadPhotoToBlobStorage(userId, photoForBS, extension, ref userPhoto));

            t1.Start();
            t2.Start();

            Task.WhenAll(t1, t2).Wait();

            userPhoto.tags = t1.Result;
            userPhoto._userId = userId;

            DBManager dbm = new DBManager();
            dbm.photoManager.addPhotoToMongoDB(userPhoto);
        }
        private static void uploadPhotoToBlobStorage(string userId, Stream photo, string extension, ref Photo userPhoto)
        {
            userPhoto._id = ObjectId.GenerateNewId();
            string photoName = userPhoto._id + extension;
            userPhoto.photoPhat = string.Format("https://{0}.blob.core.windows.net/{1}/{2}", Variables.ACCOUNT_NAME_FOR_BLOB_STORAGE, userId, photoName);

            ConnectionBS cbs = new ConnectionBS(userId);
            cbs.userBSManager.addPhotoToUserContainer(photo, photoName);
        }
        #endregion
    }
}