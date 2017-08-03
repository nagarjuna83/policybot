namespace AppInsightsBot
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Newtonsoft.Json;
    using AzureSearchBot.Services;
    using AppInsightsBot.Services;
    using System.Linq;

    [Serializable]
    public class StateDialog : IDialog<object>
    {
        private bool userWelcomed;

        private readonly TextAnalyticsService analyticsService = new TextAnalyticsService();
        private readonly QnAService qnaService = new QnAService();

        public async Task StartAsync(IDialogContext context)
        {
            var telemetry = context.CreateTraceTelemetry(nameof(StartAsync), new Dictionary<string, string> { { @"SetDefault", bool.FalseString } });

            await context.PostAsync($"Welcome to the Policy City bot. I'm currently configured to answer questions about neudesic policies");

            WebApiApplication.Telemetry.TrackTrace(telemetry);

            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            // Here's how we can serialize an entire object to an App Insights event
            WebApiApplication.Telemetry.TrackTrace(context.CreateTraceTelemetry(
                nameof(MessageReceivedAsync),
                new Dictionary<string, string> { { "message", JsonConvert.SerializeObject(message.Text) } }));

            string userName;

            if (!context.UserData.TryGetValue(ContextConstants.UserNameKey, out userName))
            {
                var t = context.CreateEventTelemetry(@"new user");
                t.Properties.Add("userName", userName); // You can add properties after-the-fact as well

                WebApiApplication.Telemetry.TrackEvent(t);

                PromptDialog.Text(context, this.ResumeAfterPrompt, "Before get started, please tell me your name?");
                return;
            }

            if (!this.userWelcomed)
            {
                this.userWelcomed = true;
                await context.PostAsync($"Welcome back {userName}!");

                context.Wait(this.MessageReceivedAsync);
                return;
            }

            var measuredEvent = context.CreateEventTelemetry(@"question");
            var timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            try
            {
                var queryDocument = await analyticsService.GetKeyPhrases(message.Text);
                var keyPhrases = queryDocument.documents.SelectMany(x => x.KeyPhrases);
                var searchResult = await qnaService.QueryQnABot(keyPhrases.Any(x => x != string.Empty) ? String.Join(",", keyPhrases) : message.Text);
                if (searchResult.Score == 0)
                {
                    WebApiApplication.Telemetry.TrackEvent(context.CreateEventTelemetry("FailedToAnswer", new Dictionary<string, string> { { "message", JsonConvert.SerializeObject(message.Text) } }));
                }
                await context.PostAsync(searchResult.Answer);

            }
            catch (Exception ex)
            {
                measuredEvent.Properties.Add("exception", ex.ToString());
                WebApiApplication.Telemetry.TrackException(context.CreateExceptionTelemetry(ex));
            }
            finally
            {
                timer.Stop();
                measuredEvent.Metrics.Add(@"timeTakenMs", timer.ElapsedMilliseconds);

                WebApiApplication.Telemetry.TrackEvent(measuredEvent);
            }
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task ResumeAfterPrompt(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var userName = await result;
                this.userWelcomed = true;

                await context.PostAsync($"Welcome {userName}!");

                context.UserData.SetValue(ContextConstants.UserNameKey, userName);
            }
            catch (TooManyAttemptsException ex)
            {
                WebApiApplication.Telemetry.TrackException(context.CreateExceptionTelemetry(ex));
            }
            finally
            {
                // It's a good idea to log telemetry in finally {} blocks so you don't end up with gaps of execution
                // as you follow a conversation
                WebApiApplication.Telemetry.TrackTrace(context.CreateTraceTelemetry(nameof(ResumeAfterPrompt)));
            }

            context.Wait(this.MessageReceivedAsync);
        }
    }
}
