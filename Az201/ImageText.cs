using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Az201
{
    class ImageText
    { 
        private const string subscriptionKey = "64002ef54cf54d8fa604618f530ddc96";
        private const string endpoint = "https://contosocvana.cognitiveservices.azure.com/";
        private const string localImagePath = @"C:\Users\gilar\Downloads\sampleimages\sampleimages\sample2.jpg";
        private const string remoteImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/d/dd/Cursive_Writing_on_Notebook_paper.jpg/800px-Cursive_Writing_on_Notebook_paper.jpg";
        public void ImageHandText()
        {
            ComputerVisionClient computerVision = new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionKey), new System.Net.Http.DelegatingHandler[] { });
            computerVision.Endpoint = endpoint;
            var t1 = ExtractRemoteHandTextAsync(computerVision, remoteImageUrl);
            var t2 = ExtractLocalHandTextAsync(computerVision, localImagePath);
            Task.WhenAll(t1, t2).Wait(5000);
            Console.WriteLine("Press any key to exit.");
            Console.Read();
        }
        private static async Task ExtractRemoteHandTextAsync(ComputerVisionClient computerVision, string imageUrl)
        {
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                Console.WriteLine("\nInvalid image:\n{0} \n", imageUrl);
                return;
            }
            RecognizeTextHeaders textHeaders = await computerVision.RecognizeTextAsync(imageUrl, TextRecognitionMode.Handwritten);
            await GetTextAsync(computerVision, textHeaders.OperationLocation);
        }
        private static async Task ExtractLocalHandTextAsync(ComputerVisionClient computerVision, string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                Console.WriteLine("\nInvalid image:\n{0} \n",
                imagePath);
                return;
            }
            using (Stream imageStream = File.OpenRead(imagePath))
            {
                RecognizeTextInStreamHeaders textHeaders = await computerVision.RecognizeTextInStreamAsync(imageStream, TextRecognitionMode.Printed);
                await GetTextAsync(computerVision, textHeaders.OperationLocation);
            }
        }
        private static async Task GetTextAsync(ComputerVisionClient computerVision, string operationLocation)
        {
            string operationId = operationLocation.Substring(operationLocation.Length - 36);
            TextOperationResult result = await computerVision.GetTextOperationResultAsync(operationId);
            int i = 0;
            int maxRetries = 10;
            while ((result.Status == TextOperationStatusCodes.Running || result.Status == TextOperationStatusCodes.NotStarted) && i++ < maxRetries)
            {
                Console.WriteLine("Server status: {0}, waiting {1} seconds...", result.Status, i);
                await Task.Delay(i*1000);
                result = await computerVision.GetTextOperationResultAsync(operationId);
            }
            Console.WriteLine();
            var lines = result.RecognitionResult.Lines;
            foreach (Line line in lines)
            {
                Console.WriteLine(line.Text);
            }
            Console.WriteLine();
        }
    }
}
