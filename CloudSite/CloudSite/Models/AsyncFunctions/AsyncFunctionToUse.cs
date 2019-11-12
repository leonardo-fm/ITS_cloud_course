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
using System.Drawing.Imaging;
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
        public static void RemoveImages(string userId ,List<string> listOfTheNamesOfPhotosToBeRemoveWithExtension)
        {
            DBManager dbm = new DBManager();
            ConnectionBS cbs = new ConnectionBS(userId);

            List<string> photosNameNoExtension = new List<string>();
            foreach (string nameWithExtesion in listOfTheNamesOfPhotosToBeRemoveWithExtension)
            {
                int indexOfPoint = nameWithExtesion.IndexOf('.');
                photosNameNoExtension.Add(nameWithExtesion.Substring(0, indexOfPoint));
                LogManager.WriteOnLog("user " + userId + " have delited an image with name " + nameWithExtesion);
            }

            Task removeFromMongoDB = new Task(() => dbm.photoManager.RemovePhotos(photosNameNoExtension));
            Task RemoveFromBlobStorage = new Task(() => cbs.userBSManager.RemovePhotoFromBlobStorage(listOfTheNamesOfPhotosToBeRemoveWithExtension));

            removeFromMongoDB.Start();
            RemoveFromBlobStorage.Start();
        }

        public static void SendMailForConvalidation(User user)
        {
            Task sendNewEmail = new Task(() =>
            {
                DBManager dbm = new DBManager();
                ConvalidationUser cu = new ConvalidationUser(user);

                user.userPassword = cu.CryptUserPassword(user.userPassword);

                dbm.userManager.AddUserToMongoDB(user);

                DefaultBodyText df = new DefaultBodyText(user.userName, user._id);
                string text = df.GetNewBodyForEmailSubscription();
                SendMail sm = new SendMail();
                sm.SendNewEmail(user.userEmail, text);
            });

            sendNewEmail.Start();
        }

        #region UploadPhoto
        public static void UploadPhoto(HttpPostedFileBase file, string userId)
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

                PropertyItem[] exifArray = Image.FromStream(photo).PropertyItems;

                Task sendImageToComputerVisionAndBlobStorage = new Task(() => SendImageToCVandBS(userPhoto, photoForCV, photoForBS, userId, extension, exifArray));
                sendImageToComputerVisionAndBlobStorage.Start();
            }
        }

        private static void SendImageToCVandBS(Photo userPhoto, Stream photoForCV, Stream photoForBS, string userId, string extension, PropertyItem[] exifArray)
        {
            Task<string[]> sendImageToComputerVision = new Task<string[]>(() => ComputerVisionConnection.UploadImageAndHandleTagsResoult(photoForCV));
            Task uploadImageToBlobStorage = new Task(() => UploadPhotoToBlobStorage(userId, photoForBS, extension, ref userPhoto));

            sendImageToComputerVision.Start();
            uploadImageToBlobStorage.Start();

            Task.WhenAll(sendImageToComputerVision, uploadImageToBlobStorage).Wait();

            userPhoto.tags = sendImageToComputerVision.Result;
            userPhoto._userId = userId;
            userPhoto.photoTimeOfUpload = string.Format("{0}:{1}:{2} {3}:{4}:{5}",
                DateTime.Now.Year, DateTime.Now.Month.ToString("d2"), DateTime.Now.Day.ToString("d2"),
                DateTime.Now.Hour.ToString("d2"), DateTime.Now.Minute.ToString("d2"), DateTime.Now.Second.ToString("d2"));

            DBManager dbm = new DBManager();

            Dictionary<int, byte[]> exifDictionary = exifArray.ToDictionary(x => x.Id, x => x.Value != null ? x.Value : new byte[] { });

            userPhoto.photoGpsLatitude = exifDictionary.ContainsKey(0x0002) ? (double?)GetGPSValues(exifDictionary[0x0002]) : null;
            userPhoto.photoGpsLongitude = exifDictionary.ContainsKey(0x0004) ? (double?)GetGPSValues(exifDictionary[0x0004]) : null;
            userPhoto.photoTagDateTime = exifDictionary.ContainsKey(0x0132) ? Encoding.UTF8.GetString(exifDictionary[0x0132]).Replace("\0", "") : "";
            userPhoto.photoTagThumbnailEquipModel = exifDictionary.ContainsKey(0x010F) && exifDictionary.ContainsKey(0x0110) ?
                Encoding.UTF8.GetString(exifDictionary[0x010F]).Replace("\0", "")
                + "/" + Encoding.UTF8.GetString(exifDictionary[0x0110]).Replace("\0", "") : "";
            userPhoto.photoTagImageWidth = exifDictionary.ContainsKey(0x0100) ? BitConverter.ToInt16(exifDictionary[0x0100], 0).ToString() : "";
            userPhoto.photoTagImageHeight = exifDictionary.ContainsKey(0x0101) ? BitConverter.ToInt16(exifDictionary[0x0101], 0).ToString(): "";

            dbm.photoManager.AddPhotoToMongoDB(userPhoto);
        }

        private static double GetGPSValues(byte[] value)
        {
            byte[] degrees1 = new byte[] { value[0], value[1], value[2], value[3] };
            byte[] degrees2 = new byte[] { value[4], value[5], value[6], value[7] };

            byte[] first1 = new byte[] { value[8], value[9], value[10], value[11] };
            byte[] first2 = new byte[] { value[12], value[13], value[14], value[15] };

            byte[] second1 = new byte[] { value[16], value[17], value[18], value[19] };
            byte[] second2 = new byte[] { value[20], value[21], value[22], value[23] };

            double degrees = (double)BitConverter.ToInt32(degrees1, 0) / BitConverter.ToInt32(degrees2, 0);
            double firsts = (double)BitConverter.ToInt32(first1, 0) / BitConverter.ToInt32(first2, 0);
            double seconds = (double)BitConverter.ToInt32(second1, 0) / BitConverter.ToInt32(second2, 0);

            return Math.Round(degrees + (firsts / 60) + (seconds / 3600), 5);
        }

        private static void UploadPhotoToBlobStorage(string userId, Stream photo, string extension, ref Photo userPhoto)
        {
            userPhoto._id = ObjectId.GenerateNewId();
            string photoName = userPhoto._id + extension;
            userPhoto.photoPhat = string.Format("https://{0}.blob.core.windows.net/{1}/{2}", Variables.ACCOUNT_NAME_FOR_BLOB_STORAGE, userId, photoName);

            ConnectionBS cbs = new ConnectionBS(userId);
            cbs.userBSManager.AddPhotoToUserContainer(photo, photoName);
        }
        #endregion
    }
}