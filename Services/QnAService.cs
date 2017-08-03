using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AppInsightsBot.Services
{
    [Serializable]
    public class QnAService
    {
        public  async Task<QnAQuery> QueryQnABot(string Query)
        {
            QnAQuery QnAQueryResult = new QnAQuery();
            using (System.Net.Http.HttpClient client =
                new System.Net.Http.HttpClient())
            {
                string RequestURI = String.Format("{0}{1}{2}{3}{4}",
                    @"https://westus.api.cognitive.microsoft.com/",
                    @"qnamaker/v1.0/",
                    @"knowledgebases/",
                    "2a4c33cc-b5ab-45c9-b1d3-345ef2f665b2",
                    @"/generateAnswer");
                var httpContent =
                    new StringContent($"{{\"question\": \"{Query}\"}}",
                    Encoding.UTF8, "application/json");
                httpContent.Headers.Add(
                    "Ocp-Apim-Subscription-Key", "da6a5d9566ad47dba0fe275c79970bf1");
                System.Net.Http.HttpResponseMessage msg =
                    await client.PostAsync(RequestURI, httpContent);
                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse =
                        await msg.Content.ReadAsStringAsync();
                    QnAQueryResult =
                        JsonConvert.DeserializeObject<QnAQuery>(JsonDataResponse);
                }
            }
            return QnAQueryResult;
        }
    }
}