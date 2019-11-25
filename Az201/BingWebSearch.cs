using System;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Generic;

namespace Az201
{
    class BingWebSearch
    {
        private const string accessKey = "50e3c7b55e144762aa31c9d5a156eadb";
        private const string uriBase = "https://contososearchana.cognitiveservices.azure.com/bing/v7.0/search";
        //const string searchTerm = "Weather in Italy";
        const string searchTerm = "Clima en Mexico+site:google.com";
        struct SearchResult
        {
            public string jsonResult;
            public Dictionary<string, string> relevantHeaders;
        }
        public void BingSearchApi()
        {
            Console.OutputEncoding = Encoding.UTF8;
            if (accessKey.Length == 32)
            {
                Console.WriteLine("Searching the Web for: " + searchTerm);
                SearchResult result = BingSearch(searchTerm);

                Console.WriteLine("\nRelevant HTTP Headers:\n");
                foreach (var header in result.relevantHeaders)
                {
                    Console.WriteLine(header.Key + ": " + header.Value);
                }
                Console.WriteLine("\nJSON Response:\n");
                Console.WriteLine(JsonPrettyPrint(result.jsonResult));
            }
            else
            {
                Console.WriteLine("Invalid Bing Search API subscription key!");
            }
            Console.WriteLine("Press any key to exit.");
            Console.Read();
        }
        static SearchResult BingSearch(string searchQuery)
        {
            var uriQuery = uriBase + "?q=" + Uri.EscapeDataString(searchQuery);
            WebRequest request = WebRequest.Create(uriQuery);
            request.Headers["Ocp-Apim-Subscription-Key"] = accessKey;
            HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result;
            string json = new StreamReader(response.GetResponseStream()).ReadToEnd();
            var searchResult = new SearchResult()
            {
                jsonResult = json,
                relevantHeaders = new Dictionary<string, string>()
            };
            foreach (string header in response.Headers)
            {
                if (header.StartsWith("BingAPIs-") || header.StartsWith("XMSEdge-"))
                {
                    searchResult.relevantHeaders[header] = response.Headers[header];
                }
            }
            return searchResult;
        }
        static string JsonPrettyPrint(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return string.Empty;
            }
            json = json.Replace(Environment.NewLine, "").Replace("\t", "");
            StringBuilder sb = new StringBuilder();
            bool quote = false;
            bool ignore = false;
            char last = ' ';
            int offset = 0;
            int indentLength = 2;
            foreach (char ch in json)
            {
                switch (ch)
                {
                    case '"':
                        if (!ignore) quote = !quote;
                        break;
                    case '\\':
                        if (quote && last != '\\') ignore = true;
                        break;
                }
                if (quote)
                {
                    sb.Append(ch);
                    if (last == '\\' && ignore) ignore = false;
                }
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
                            if (quote || ch != ' ') sb.Append(ch);
                            break;
                    }
                }
                last = ch;
            }
            return sb.ToString().Trim();
        }
    }
}
