using CloudSite.Models.ConvalidationUserAuth;
using CloudSite.Models.ComputerVision;
using CloudSite.Models.EmailSender;
using CloudSite.Models.BlobStorage;
using CloudSite.Models.LogManager;
using CloudSite.Models.MoongoDB;
using CloudSite.Models.Photos;
using CloudSite.Models.User;
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
        public static void RemoveImages(string userId, List<string> listOfTheNamesOfPhotosToBeRemoveWithExtension)
        {
            DBManager dbm = new DBManager();
            ConnectionBS cbs = new ConnectionBS(userId);

            List<string> photosNameNoExtension = new List<string>();
            foreach (string nameWithExtesion in listOfTheNamesOfPhotosToBeRemoveWithExtension)
            {
                int indexOfPoint = nameWithExtesion.IndexOf('.');

                if(!nameWithExtesion.Contains("_Preview"))
                    photosNameNoExtension.Add(nameWithExtesion.Substring(0, indexOfPoint));

                // Log
                LogMaster.WriteOnLog("user " + userId + " have deleted an image with name " + nameWithExtesion);
            }

            Task removeFromMongoDB = new Task(() => dbm.PhotoManager.RemovePhotos(photosNameNoExtension));
            Task RemoveFromBlobStorage = new Task(
                () => cbs.UserBSManager.RemovePhotoFromBlobStorage(listOfTheNamesOfPhotosToBeRemoveWithExtension));

            removeFromMongoDB.Start();
            RemoveFromBlobStorage.Start();

            Task.WhenAll(removeFromMongoDB, RemoveFromBlobStorage).Wait();
        }

        public static void SendMailForConvalidation(UserModel user)
        {
            Task sendNewEmail = new Task(() =>
            {
                DBManager dbm = new DBManager();
                ConvalidationUser cu = new ConvalidationUser(user);

                user.UserPassword = cu.CryptUserPassword(user.UserPassword);

                dbm.UserManager.AddUserToMongoDB(user);

                DefaultBodyText df = new DefaultBodyText(user.UserName, user._id);
                string text = df.GetNewBodyForEmailSubscription();
                SendMail sm = new SendMail();
                sm.SendNewEmail(user.UserEmail, text);
            });

            sendNewEmail.Start();
        }

        #region UploadPhoto
        public static void UploadPhoto(HttpPostedFileBase file, string userId)
        {
            Photo userPhoto = new Photo();

            userPhoto.ImageName = file.FileName;

            string extension = file.FileName.Substring(file.FileName.IndexOf('.'));

            Stream photoForComputerVision = new MemoryStream();
            Stream photoForBlobStorageOriginalSize = new MemoryStream();
            Stream photoForBlobStoragePreview = new MemoryStream();
            Stream photoForBlobStoragePreviewClear = new MemoryStream();

            using (Stream photo = file.InputStream)
            {
                EncoderParameters ep = new EncoderParameters();
                ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Png);

                photo.CopyTo(photoForComputerVision);
                photoForComputerVision.Seek(0, SeekOrigin.Begin);

                photo.Seek(0, SeekOrigin.Begin);

                photo.CopyTo(photoForBlobStorageOriginalSize);
                photoForBlobStorageOriginalSize.Seek(0, SeekOrigin.Begin);

                photo.Seek(0, SeekOrigin.Begin);

                photo.CopyTo(photoForBlobStoragePreview);
                photoForBlobStoragePreview.Seek(0, SeekOrigin.Begin);

                photo.Seek(0, SeekOrigin.Begin);
                Image photoForExif = Image.FromStream(photo);
                PropertyItem[] exifArray = photoForExif.PropertyItems;
                userPhoto.photoTagImageWidth = photoForExif.Width.ToString();
                userPhoto.photoTagImageHeight = photoForExif.Height.ToString();

                /* Compress image */
                var imageToBeCompress = Image.FromStream(photoForBlobStoragePreview);
                photoForBlobStoragePreview.Seek(0, SeekOrigin.Begin);

                var photoPreview = ResizeImage(imageToBeCompress);

                ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 0L);

                photoPreview.Save(photoForBlobStoragePreviewClear, jgpEncoder, ep);
                photoForBlobStoragePreviewClear.Seek(0, SeekOrigin.Begin);

                Task sendImageToComputerVisionAndBlobStorage = new Task(
                    () => SendImageToCVandBS(userPhoto, photoForComputerVision, photoForBlobStorageOriginalSize, photoForBlobStoragePreviewClear, userId, extension, exifArray));
                sendImageToComputerVisionAndBlobStorage.Start();
                sendImageToComputerVisionAndBlobStorage.Wait();
            }
        }

        private static Bitmap ResizeImage(Image image)
        {
            Bitmap destImage = new Bitmap(image, new Size(300, 169));

            return destImage;
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private static void SendImageToCVandBS(
            Photo userPhoto, Stream photoForCV, Stream photoForBSOS, Stream photoForBSP, string userId, string extension, PropertyItem[] exifArray
            )
        {
            Task<string[]> sendImageToComputerVision = new Task<string[]>(() => ComputerVisionConnection.UploadImageAndHandleTagsResoult(photoForCV));
            Task uploadImageOSToBlobStorage = new Task(() => UploadPhotoOSToBlobStorage(userId, photoForBSOS, extension, ref userPhoto));
            Task uploadImagePToBlobStorage = new Task(() => UploadPhotoPToBlobStorage(userId, photoForBSP, extension, ref userPhoto));

            sendImageToComputerVision.Start();

            uploadImageOSToBlobStorage.Start();
            // We have to wait bacause we need the _id for the photoPreview
            uploadImageOSToBlobStorage.Wait();

            uploadImagePToBlobStorage.Start();

            Task.WhenAll(sendImageToComputerVision, uploadImagePToBlobStorage).Wait();

            userPhoto.Tags = sendImageToComputerVision.Result;
            userPhoto.UserId = userId;
            userPhoto.PhotoTimeOfUpload = string.Format("{0}:{1}:{2} {3}:{4}:{5}",
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
            //userPhoto.photoTagImageWidth = exifDictionary.ContainsKey(0x0100) ? BitConverter.ToInt16(exifDictionary[0x0100], 0).ToString() : "";
            //userPhoto.photoTagImageHeight = exifDictionary.ContainsKey(0x0101) ? BitConverter.ToInt16(exifDictionary[0x0101], 0).ToString() : "";

            dbm.PhotoManager.AddPhotoToMongoDB(userPhoto);
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

        private static void UploadPhotoOSToBlobStorage(string userId, Stream photo, string extension, ref Photo userPhoto)
        {
            userPhoto._id = ObjectId.GenerateNewId();

            string photoName = userPhoto._id + extension;
            userPhoto.PhotoNameOriginalSize = photoName;

            userPhoto.PhotoPhatOriginalSize = string.Format("https://{0}.blob.core.windows.net/{1}/{2}", 
                Variables.ACCOUNT_NAME_FOR_BLOB_STORAGE, userId, photoName);

            ConnectionBS cbs = new ConnectionBS(userId);
            cbs.UserBSManager.AddPhotoToUserContainer(photo, photoName);
        }

        private static void UploadPhotoPToBlobStorage(string userId, Stream photo, string extension, ref Photo userPhoto)
        {
            string photoName = userPhoto._id.ToString() + "_Preview" + extension;
            userPhoto.PhotoNamePreview = photoName;

            userPhoto.PhotoPhatPreview = string.Format("https://{0}.blob.core.windows.net/{1}/{2}", 
                Variables.ACCOUNT_NAME_FOR_BLOB_STORAGE, userId, photoName);

            ConnectionBS cbs = new ConnectionBS(userId);
            cbs.UserBSManager.AddPhotoToUserContainer(photo, photoName);
        }
        #endregion
    }
}