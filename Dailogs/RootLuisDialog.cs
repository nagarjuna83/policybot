

namespace LuisBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.FormFlow;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Connector;
    using System.Net.Mail;
    using System.Threading;
    using AppInsightsBot.Services;
    using AppInsightsBot;
    using Newtonsoft.Json;

    [LuisModel("04808423-d5b5-45fe-87bf-4946b38c14bc", "9f69440c3add42219149bc256245118d")]
    [Serializable]
    public class RootLuisDialog : LuisDialog<object>
    {
        private readonly TextAnalyticsService analyticsService = new TextAnalyticsService();
        private readonly QnAService qnaService = new QnAService();

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            QnAQuery searchResult = await GetAnswer(context, result.Query, true);
            await context.PostAsync(searchResult.Answer);

            context.Wait(this.MessageReceived);
        }
        [LuisIntent("Welcome")]
        public async Task Welcome(IDialogContext context, LuisResult result)
        {
            //if (ConversationHelper.IsConversationStarted(context))
            //{
            //    await context.PostAsync("You are middle of the conversation. do you want to reset the conversation ?");
            //}
            //else
            //{
            var name = context.Activity.From.Name;
            await context.PostAsync("Hi " + name + ".");
            var telemetry = context.CreateTraceTelemetry(nameof(StartAsync), new Dictionary<string, string> { { @"SetDefault", bool.FalseString } });

            await context.PostAsync($"Welcome to the Policy City bot. I'm currently configured to answer questions about neudesic policies");

            WebApiApplication.Telemetry.TrackTrace(telemetry);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("DressCode")]
        public async Task DressCode(IDialogContext context, LuisResult result)
        {
            var response = await this.GetAnswer(context, "Dress Code");
            await context.PostAsync(response.Answer);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("TimeSheet")]
        public async Task TimeSheet(IDialogContext context, LuisResult result)
        {
            var response = await this.GetAnswer(context, "time sheet");
            await context.PostAsync(response.Answer);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Salary")]
        public async Task Salary(IDialogContext context, LuisResult result)
        {
            var response = await this.GetAnswer(context, "time sheet");
            await context.PostAsync(response.Answer);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Onsite")]
        public async Task Onsite(IDialogContext context, LuisResult result)
        {
            var response = await this.GetAnswer(context, "on site");
            await context.PostAsync(response.Answer);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Leave")]
        public async Task Leaves(IDialogContext context, LuisResult result)
        {
            var response = await this.GetAnswer(context, "Leaves");
            await context.PostAsync(response.Answer);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Holidays")]
        public async Task Holidays(IDialogContext context, LuisResult result)
        {
            var response = await this.GetAnswer(context, "Holidays");
            await context.PostAsync(response.Answer);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Travel")]
        public async Task Travel(IDialogContext context, LuisResult result)
        {
            var response = await this.GetAnswer(context, "Travel");
            await context.PostAsync(response.Answer);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Welcome to the Policy City bot. I'm currently configured to answer questions about neudesic policies");
            context.Wait(this.MessageReceived);
        }

        
        private async Task<QnAQuery> GetAnswer(IDialogContext context, string message, bool checkKeyPhrases = false)
        {
            var queryDocument = await analyticsService.GetKeyPhrases(message);
            IEnumerable<string> keyPhrases = new List<string>();
            if (checkKeyPhrases)
            {
                keyPhrases = queryDocument.documents.SelectMany(x => x.KeyPhrases);
            }
            QnAQuery searchResult = await qnaService.QueryQnABot(keyPhrases.Any(x => x != string.Empty) ? String.Join(",", keyPhrases) : message);
            if (searchResult.Score == 0)
            {
                WebApiApplication.Telemetry.TrackEvent(context.CreateEventTelemetry("FailedToAnswer", new Dictionary<string, string> { { "message", JsonConvert.SerializeObject(message) } }));
            }

            return searchResult;
        }

       
    }
}
