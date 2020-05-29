// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.9.1

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using System.Web;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Rin.Bots
{
    public class EchoBot : ActivityHandler
    {
        public LuisRecognizer rin { get; private set; }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {


            if (string.Equals(turnContext.Activity.Text, "wait", System.StringComparison.InvariantCultureIgnoreCase))
            {
                await turnContext.SendActivitiesAsync(
                    new Activity[] {
                new Activity { Type = ActivityTypes.Typing },
                new Activity { Type = "delay", Value= 3000 },
                MessageFactory.Text("Finished typing", "Finished typing"),
                    },
                    cancellationToken);
            }
            else
            { 
                string url = "https://westus.api.cognitive.microsoft.com/luis/prediction/v3.0/apps/8c4ca628-d320-44fe-bff3-68fa687a97d6/slots/staging/predict?subscription-key=efe57775900d488196a432a0e93304d5&verbose=true&show-all-intents=false&log=true&query=";
                var client = new HttpClient();
                var response = await client.GetAsync(url+turnContext.Activity.Text);
                var strResponseContent = await response.Content.ReadAsStringAsync();
                JObject applyJObj = JObject.Parse(strResponseContent);
                string topIntent = applyJObj["prediction"]["topIntent"].ToString();
                /*
                string applicationId = "8c4ca628-d320-44fe-bff3-68fa687a97d6";
                string endpointKey = "efe57775900d488196a432a0e93304d5";
                string endpoint = "https://westus.api.cognitive.microsoft.com";
                
                var LuisforRin = new LuisApplication(applicationId,endpointKey,endpoint);
                var recognizerOptions = new LuisRecognizerOptionsV2(LuisforRin)
                {
                    PredictionOptions = new LuisPredictionOptions()
                    {
                        IncludeInstanceData = true
                    }
                };
                rin = new LuisRecognizer(recognizerOptions);
               
                var replyText = $"Echo: {turnContext.Activity.Text}. Say 'wait' to watch me type.";
                
                var recognizerResult = await rin.RecognizeAsync(turnContext, cancellationToken);
                var topIntent = recognizerResult.GetTopScoringIntent();
                */
                switch (topIntent)
                {
                    case "greeting":
                        await turnContext.SendActivityAsync(MessageFactory.Text("안녕하세요!"), cancellationToken);
                        break;
                    default:
                        await turnContext.SendActivityAsync(MessageFactory.Text("음..잘 모르겠어요!"), cancellationToken);
                        break;


                }
                
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            
            var welcomeText = "Hello and welcome!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }
}
