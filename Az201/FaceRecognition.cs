using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Az201
{
    class FaceRecognition
    {
        private const string subscriptionKey = "3c5ce84fa38e4892b462999ce7e447b1";
        private const string uriBase = "https://myfaceana.cognitiveservices.azure.com/face/v1.0/detect";

        public void SearchFace()
        {
            Console.WriteLine("Detect faces");
            Console.Write("Enter the image path: ");
            string imageFilePath = Console.ReadLine();

            MakeAnalysisRequest(imageFilePath).Wait();
            Console.WriteLine("Press any key to exit.");
            Console.Read();
        }
        private async Task MakeAnalysisRequest(string imageFilePath)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            string requestParameters = "returnFaceId=true&returnFaceLandmarks=false&returnFaceAttributes=age,gender,smile,glasses,emotion,accessories";
            //facialHair,hair,makeup,occlusion,accessories,blur,exposure,noise";
            string uri = uriBase + "?" + requestParameters;
            HttpResponseMessage response;
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);
                string contentString = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JsonPrint(contentString));
            }
        }
        private static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }
        private static string JsonPrint(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return string.Empty;
            }
            json = json.Replace(Environment.NewLine, "").Replace("\t", "");
            StringBuilder sb = new StringBuilder();
            bool quote = false;
            bool ignore = false;
            int offset = 0;
            int indentLength = 3;
            foreach (char ch in json)
            {
                switch (ch)
                {
                    case '"':
                        if (!ignore) quote = !quote;
                        break;
                    case '\'':
                        if (quote) ignore = !ignore;
                        break;
                }

                if (quote)
                    sb.Append(ch);
                else
                {
                    switch (ch)
                    {
                        case '{':
                        case '[':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', ++offset * indentLength));
                            break;
                        case '}':
                        case ']':
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', --offset * indentLength));
                            sb.Append(ch);
                            break;
                        case ',':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', offset * indentLength));
                            break;
                        case ':':
                            sb.Append(ch);
                            sb.Append(' ');
                            break;
                        default:
                            if (ch != ' ') sb.Append(ch);
                            break;
                    }
                }
            }
            return sb.ToString().Trim();
        }
    }
}
