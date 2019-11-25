using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Az201
{
    class ComputerVision
    {
        private const string subscriptionKey = "64002ef54cf54d8fa604618f530ddc96";
        private const string endpoint = "https://contosocvana.cognitiveservices.azure.com/";
        private const string localImagePath = @"C:\Users\gilar\Downloads\sampleimages\sampleimages\sample7.png";
        private const string remoteImageUrl = "http://upload.wikimedia.org/wikipedia/commons/3/3c/Shaki_waterfall.jpg";

        private static readonly List<VisualFeatureTypes> features = new List<VisualFeatureTypes>()
        {
            VisualFeatureTypes.Categories,
            VisualFeatureTypes.Description,
            VisualFeatureTypes.Faces,
            VisualFeatureTypes.ImageType,
            VisualFeatureTypes.Objects

        };

        public void ImageAnalyze()
        {
            ComputerVisionClient computerVision = new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionKey), new System.Net.Http.DelegatingHandler[] { });
            computerVision.Endpoint = endpoint;
            var i1 = AnalyzeRemoteAsync(computerVision, remoteImageUrl);
            var i2 = AnalyzeLocalAsync(computerVision, localImagePath);
            Task.WhenAll(i1, i2).Wait(5000);
            Console.WriteLine("Press any key to exit.");
            Console.Read();
        }
        private static async Task AnalyzeRemoteAsync(ComputerVisionClient computerVision, string imageUrl)
        {
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                Console.WriteLine("\nInvalid file:\n{0} \n", imageUrl);
                return;
            }
            ImageAnalysis analysis = await computerVision.AnalyzeImageAsync(imageUrl, features);
            DisplayResults(analysis, imageUrl);
        }
        private static async Task AnalyzeLocalAsync(ComputerVisionClient computerVision, string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                Console.WriteLine("\nInvalid file:\n{0} \n", imagePath);
                return;
            }
            using (Stream imageStream = File.OpenRead(imagePath))
            {
                ImageAnalysis analysis = await computerVision.AnalyzeImageInStreamAsync(imageStream, features);
                DisplayResults(analysis, imagePath);
            }
        }
        private static void DisplayResults(ImageAnalysis analysis, string imageUri)
        {
            Console.WriteLine(imageUri);
            foreach (var capt in analysis.Objects)
            {
                Console.WriteLine(capt.ObjectProperty + "\n");
            }
        }
    }
}
