using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Az201
{
    class Thumbnail
    {
        private const bool writeThumbnailToDisk = true;
        private const string subscriptionKey = "64002ef54cf54d8fa604618f530ddc96";
        private const string endpoint = "https://contosocvana.cognitiveservices.azure.com/";
        private const string localImagePath = @"C:\Users\gilar\Downloads\sampleimages\sampleimages\sample16.png";
        private const string remoteImageUrl = "https://upload.wikimedia.org/wikipedia/commons/9/94/Bloodhound_Puppy.jpg";
        private const int thumbnailWidth = 100;
        private const int thumbnailHeight = 100;
        public void ImageThumbnail()
        {
            ComputerVisionClient computerVision = new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionKey), new System.Net.Http.DelegatingHandler[] { });
            computerVision.Endpoint = endpoint;
            var t1 = GetRemoteThumbnailAsync(computerVision, remoteImageUrl);
            var t2 = GetLocalThumbnailAsnc(computerVision, localImagePath);
            Task.WhenAll(t1, t2).Wait(5000);
            Console.WriteLine("Press any key to exit.");
            Console.Read();
        }
        private static async Task GetRemoteThumbnailAsync(ComputerVisionClient computerVision, string imageUrl)
        {
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                Console.WriteLine("\nInvalid image:\n{0} \n", imageUrl);
                return;
            }
            Stream thumbnail = await computerVision.GenerateThumbnailAsync(thumbnailWidth, thumbnailHeight, imageUrl, false);
            string path = localImagePath.Substring(0, localImagePath.LastIndexOf('\\'));
            string imageName = imageUrl.Substring(imageUrl.LastIndexOf('/') + 1);
            string thumbnailFilePath = path + "\\" + imageName.Insert(imageName.Length - 4, "_thumb");
            SaveThumbnail(thumbnail, thumbnailFilePath);
        }
        private static async Task GetLocalThumbnailAsnc(ComputerVisionClient computerVision, string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                Console.WriteLine("\nInvalid image:\n{0} \n", imagePath);
                return;
            }
            using (Stream imageStream = File.OpenRead(imagePath))
            {
                Stream thumbnail = await computerVision.GenerateThumbnailInStreamAsync(thumbnailWidth, thumbnailHeight, imageStream, false);
                string thumbnailFilePath = localImagePath.Insert(localImagePath.Length - 4, "_thumb");
                SaveThumbnail(thumbnail, thumbnailFilePath);
            }
        }
        private static void SaveThumbnail(Stream thumbnail, string thumbnailFilePath)
        {
            if (writeThumbnailToDisk)
            {
                using (Stream file = File.Create(thumbnailFilePath))
                {
                    thumbnail.CopyTo(file);
                }
            }
            Console.WriteLine("Thumbnail {0} saved to: {1}\n", writeThumbnailToDisk ? "" : "NOT", thumbnailFilePath);
        }
    }
}
