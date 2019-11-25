using System;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using System.Collections.Generic;
using Microsoft.Rest;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Az201
{
    class TextAnalytics
    {
        private const string subscriptionKey = "7754a59285c74eafb36ab865c239d10a";
        private const string uriBase = "https://eastus.api.cognitive.microsoft.com/";
        class ApiKeyServiceClientCredentials : ServiceClientCredentials
        {
            public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                return base.ProcessHttpRequestAsync(request, cancellationToken);
            }
        }

        public void MainTextAnalytics()
        {
            ITextAnalyticsClient client = new TextAnalyticsClient(new ApiKeyServiceClientCredentials())
            {
                Endpoint = uriBase

            };
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            string text = string.Empty;
            while (text != "e")
            {
                Console.WriteLine();
                Console.WriteLine("Add a text in any language, or type e to exit.");
                text = Console.ReadLine();
                LanguageRecognition(client, text);
            }
        }
        private void LanguageRecognition(ITextAnalyticsClient client, string text)
        {
            var result = client.DetectLanguageAsync(new BatchInput(new List<Input>() { new Input("1", text) })).Result;
            foreach (var document in result.Documents)
            {
                Console.WriteLine($"Language: {document.DetectedLanguages[0].Name}");
            }
        }
        private void KeyPrhaseRecognition(ITextAnalyticsClient client, string text, string language)
        {
            KeyPhraseBatchResult result = client.KeyPhrasesAsync(new MultiLanguageBatchInput(
                        new List<MultiLanguageInput>()
                        {
                          new MultiLanguageInput(language, "1", text)
                        })).Result;
            foreach (var document in result.Documents)
            {
                Console.WriteLine("\t Key phrases:");
                foreach (string keyphrase in document.KeyPhrases)
                {
                    Console.WriteLine($"\t\t{keyphrase}");
                }
            }
        }
        private void EmotionRecognition(ITextAnalyticsClient client, string text, string language)
        {
            SentimentBatchResult result = client.SentimentAsync(new MultiLanguageBatchInput(
                        new List<MultiLanguageInput>()
                        {
                          new MultiLanguageInput(language, "1", text)
                        })).Result;
            foreach (var document in result.Documents)
            {
                Console.WriteLine($"Document ID: {document.Id} , Sentiment Score: {document.Score}");
            }
        }
    }
}
