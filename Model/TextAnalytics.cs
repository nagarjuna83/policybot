using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppInsightsBot.Model
{
    /// <summary>
    /// Class to hold result of Key Phrases call
    /// </summary>
    public class KeyPhraseResult
    {
        public List<string> KeyPhrases { get; set; }
    }

    public class Document
    {
        public string language { get; set; }
        public string id { get; set; }
        public string text { get; set; }
        public List<string> KeyPhrases { get; set; }
    }

    public class QueryDocument
    {
        public QueryDocument(List<string> input)
        {
            this.documents = new List<Document>();
            foreach (var t in input)
            {
                documents.Add(new Document { language = "en", text = t });
            }
        }
        public QueryDocument(string input)
        {
            this.documents = new List<Document>();
            documents.Add(new Document { language = "en", text = input, id = "1" });
        }
        public QueryDocument()
        {

        }
        public List<Document> documents { get; set; }

    }

    public class questionAndPhrases
    {
        public string Content { get; set; }
        public string KeyPhrase { get; set; }
        public string ID { get; set; }
    }
    /// <summary>
    /// Class to hold result of Sentiment call
    /// </summary>
    public class SentimentResult
    {
        public double Score { get; set; }
    }

    /// <summary>
    /// Class to hold result of Language detection call
    /// </summary>
    public class LanguageResult
    {
        public bool UnknownLanguage { get; set; }
        public IList<DetectedLanguage> DetectedLanguages { get; set; }
    }

    public class QnAKnowledgeBase
    {
        public Add add { get; set; }
        public Delete delete { get; set; }
    }
    public class QnaPair
    {
        public string answer { get; set; }
        public string question { get; set; }
    }

    public class Add
    {
        public IList<QnaPair> qnaPairs { get; set; }
        public IList<string> urls { get; set; }
    }

    public class Delete
    {
        public IList<QnaPair> qnaPairs { get; set; }
        public IList<string> urls { get; set; }
    }
    /// <summary>
    /// Class to hold information about a single detected language
    /// </summary>
    public class DetectedLanguage
    {
        public string Name { get; set; }

        /// <summary>
        /// This is the short ISO 639-1 standard form of representing
        /// all languages. The short form is a 2 letter representation of the language.
        /// en = English, fr = French for example
        /// </summary>
        public string Iso6391Name { get; set; }
        public double Score { get; set; }
    }
}