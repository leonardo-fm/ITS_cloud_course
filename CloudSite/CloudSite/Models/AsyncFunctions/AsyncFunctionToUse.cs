using CloudSite.Models.BlobStorage;
using CloudSite.Models.ComputerVision;
using CloudSite.Models.ConvalidationUserAuth;
using CloudSite.Models.EmailSender;
using CloudSite.Models.Log;
using CloudSite.Models.MoongoDB;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CloudSite.Models.AsyncFunctions
{
    public class AsyncFunctionToUse
    {
        /*DA IMPLEMENTARE, SERVE UNA FUNZIONE IN JS CHE PERMETTA DI TIRARE UNA LISTA DELLE IMMAGINI SELEZIONATE E LE SPEDISCA AL SERVER*/
        public static void removeImages(string userId ,List<string> listOfTheNamesOfPhotosToBeRemoveWithExtension)
        {
            DBManager dbm = new DBManager();
            ConnectionBS cbs = new ConnectionBS(userId);

            List<string> photosNameNoExtension = new List<string>();
            foreach (string nameWithExtesion in listOfTheNamesOfPhotosToBeRemoveWithExtension)
            {
                int indexOfPoint = nameWithExtesion.IndexOf('.');
                photosNameNoExtension.Add(nameWithExtesion.Substring(0, indexOfPoint));
                LogManager.writeOnLog("user " + userId + " have delited an image with name " + nameWithExtesion);
            }

            Task removeFromMongoDB = new Task(() => dbm.photoManager.removePhotos(photosNameNoExtension));
            Task RemoveFromBlobStorage = new Task(() => cbs.userBSManager.removePhotoFromBlobStorage(listOfTheNamesOfPhotosToBeRemoveWithExtension));

            removeFromMongoDB.Start();
            RemoveFromBlobStorage.Start();
        }

        public static void sendMailForConvalidation(User user)
        {
            Task sendNewEmail = new Task(() =>
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

            sendNewEmail.Start();
        }

        #region UploadPhoto
        public static void uploadPhoto(HttpPostedFileBase file, string userId)
        {
            Photo userPhoto = new Photo();
            userPhoto.imageName = file.FileName;

            string extension = file.FileName.Substring(file.FileName.IndexOf('.'));

            Stream photoForCV = new MemoryStream();
            Stream photoForBS = new MemoryStream();
            Image imageForExif = null;

            using (Stream photo = file.InputStream)
            {
                photo.CopyTo(photoForCV);

                photoForCV.Seek(0, SeekOrigin.Begin);
                photo.Seek(0, SeekOrigin.Begin);

                photo.CopyTo(photoForBS);
                photoForBS.Seek(0, SeekOrigin.Begin);
                photo.Seek(0, SeekOrigin.Begin);

                imageForExif = Image.FromStream(photo);
            }

            Task sendImageToComputerVisionAndBlobStorage = new Task(() => sendImageToCVandBS(userPhoto, photoForCV, photoForBS, userId, extension, imageForExif));
            sendImageToComputerVisionAndBlobStorage.Start();
        }

        private static void sendImageToCVandBS(Photo userPhoto, Stream photoForCV, Stream photoForBS, string userId, string extension, Image imageForExif)
        {

            Task<string[]> sendImageToComputerVision = new Task<string[]>(() => ComputerVisionConnection.uploadImageAndHandleTagsResoult(photoForCV));
            Task uploadImageToBlobStorage = new Task(() => uploadPhotoToBlobStorage(userId, photoForBS, extension, ref userPhoto));

            sendImageToComputerVision.Start();
            uploadImageToBlobStorage.Start();

            Task.WhenAll(sendImageToComputerVision, uploadImageToBlobStorage).Wait();

            userPhoto.tags = sendImageToComputerVision.Result;
            userPhoto._userId = userId;

            DBManager dbm = new DBManager();
            userPhoto.photoGpsLatitude = Encoding.UTF8.GetString(imageForExif.GetPropertyItem(0x0002).Value);
            userPhoto.photoGpsLongitude = Encoding.UTF8.GetString(imageForExif.GetPropertyItem(0x0004).Value);
            userPhoto.photoTagDateTime = Encoding.UTF8.GetString(imageForExif.GetPropertyItem(0x0132).Value);
            userPhoto.photoTagImageWidth = Encoding.UTF8.GetString(imageForExif.GetPropertyItem(0x0100).Value);
            userPhoto.photoTagImageHeight = Encoding.UTF8.GetString(imageForExif.GetPropertyItem(0x0101).Value);

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