using AzureSearchBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using System.Threading.Tasks;
using AppInsightsBot.Model;
using System.Text.RegularExpressions;
using System.Configuration;

namespace AppInsightsBot.Services
{
    [Serializable]
    public class TextAnalyticsService
    {
        enum Mode { Add, Delete };
        static string azureMLTextAnalyticsKey = "df5f6c7b30e04683ab0484d24b4f8c65 ";     // Learn more here: https://azure.microsoft.com/en-us/documentation/articles/machine-learning-apps-text-analytics/
        private const string ServiceBaseUri = "https://westus.api.cognitive.microsoft.com/";

        public async Task<QueryDocument> GetKeyPhrases(string input)
        {
            var refinedInput = RefineInput(input);
            var query = new QueryDocument(refinedInput);

            KeyPhraseResult keyPhraseResult = new KeyPhraseResult();
            using (var httpClient = new HttpClient())
            {

                httpClient.BaseAddress = new Uri(ServiceBaseUri);

                // Request headers.
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", azureMLTextAnalyticsKey);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var inputData = new List<questionAndPhrases>();
                string id = string.Empty;

                byte[] byteData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(query));

                // Detect key phrases:
                var keyPhrasesRequest = "text/analytics/v2.0/keyPhrases";
                //var response = await CallEndpoint(httpClient, uri, byteData);
                var outputData = new List<questionAndPhrases>();
                // get key phrases
                using (var getcontent = new ByteArrayContent(byteData))
                {
                    getcontent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = await httpClient.PostAsync(keyPhrasesRequest, getcontent);

                    var contentTask = await response.Content.ReadAsAsync<QueryDocument>();
                    return contentTask;
                }
            }
        }

        private string RefineInput(string input)
        {
            var wordstoRemove = ConfigurationManager.AppSettings["WordsToRemove"].ToString().Split(',').ToList();
           

            var wordMatches = Regex.Matches(input, "\\w+")
                .Cast<Match>()
                .OrderByDescending(m => m.Index);

            foreach (var m in wordMatches)
                if (wordstoRemove.Contains(m.Value))
                    input = input.Remove(m.Index, m.Length);

            return input;
        }
    }
}