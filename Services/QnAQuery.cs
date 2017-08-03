using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppInsightsBot.Services
{
    [Serializable]
    public class QnAQuery
    {
        [JsonProperty(PropertyName = "question")]
        public string Question { get; set; }
        [JsonProperty(PropertyName = "answer")]
        public string Answer { get; set; }
        [JsonProperty(PropertyName = "score")]
        public double Score { get; set; }
    }
}