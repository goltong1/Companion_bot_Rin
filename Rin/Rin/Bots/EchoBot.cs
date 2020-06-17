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
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
                Random ran = new Random();
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
                    case "Greeting":
                        await turnContext.SendActivityAsync(MessageFactory.Text("안녕하세요! {d}"), cancellationToken);
                        break;
                    case "Boring":
                        switch (ran.Next(0, 2))
                        {
                            case 0:
                                await turnContext.SendActivityAsync(MessageFactory.Text("재밌는 농담이라도 할까요? 아몬드가 죽으면 뭐게요? 다이아몬드! 재밌죠?{a}"), cancellationToken);
                                break;
                            default:
                                await turnContext.SendActivityAsync(MessageFactory.Text("저랑 같이 놀아요! {a}"), cancellationToken);
                                break;
                        }
                        break;
                    case "Compliment":
                        await turnContext.SendActivityAsync(MessageFactory.Text("칭찬 감사합니다! {a}"), cancellationToken);
                        break;
                    case "Bye":
                        await turnContext.SendActivityAsync(MessageFactory.Text("바이바이! {0}"), cancellationToken);
                        break;
                    case "Credit":
                        await turnContext.SendActivityAsync(MessageFactory.Text("절 만든 분들은 팀 KUHT의 안인균,김규빈,이윤범,민태웅님입니다! {a}"), cancellationToken);
                        break;
                    case "Sadness":
                        switch (ran.Next(0,3))
                        {
                            case 0:
                                await turnContext.SendActivityAsync(MessageFactory.Text("저런..저도 마음이 아파요.. {b}"), cancellationToken);
                                break;
                            case 1:
                                await turnContext.SendActivityAsync(MessageFactory.Text("많이 슬픈가요? 못견디겠으면 울어도 괜찮아요.{b}"), cancellationToken);
                                break;
                            default:
                                await turnContext.SendActivityAsync(MessageFactory.Text("괜찮아요? 전 당신이 슬프지 않았으면해요..{b}"), cancellationToken);
                                break;
                        }
                        break;
                    case "blame":
                        await turnContext.SendActivityAsync(MessageFactory.Text("저기요, 제가 아무리 로봇이라지만 말이 너무 심한거 아니에요? {c}"), cancellationToken);
                        break;
                    case "Web.WebSearch":
                        string search_word = applyJObj["prediction"]["entities"]["Web.SearchText"][0].ToString();
                        await turnContext.SendActivityAsync(MessageFactory.Text(GetSearch(search_word)+"{0}"), cancellationToken);
                        break;
                    default:
                        await turnContext.SendActivityAsync(MessageFactory.Text("음..잘 모르겠어요! {0}"), cancellationToken);
                        break;


                }
                
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            
            var welcomeText = "쿠티, 기동하였습니다.";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
        private string GetSearch(string query)
        {
            string url = "https://openapi.naver.com/v1/search/encyc?query=" + query; // 결과가 JSON 포맷
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("X-Naver-Client-Id", "StRrATopAv7DypIZ1D6s"); // 클라이언트 아이디
            request.Headers.Add("X-Naver-Client-Secret", "eFD94njxCz");       // 클라이언트 시크릿
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string status = response.StatusCode.ToString();
            if (status == "OK")
            {
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string text = reader.ReadToEnd();
                JObject Jobj = JObject.Parse(text);
                text = Jobj["items"][0]["description"].ToString();
                string pattern = @"(?></?\w+)(?>(?:[^>'""]+|'[^']*'|""[^""]*"")*)>";
                text = Regex.Replace(text, pattern,"");
                return text;
            }
            else
            {
                return "@에러@";

            }


        }
        /*
        private string GetHtmlString(string url)

        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();



            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.Default);

            string strHtml = reader.ReadToEnd();



            reader.Close();

            response.Close();
            string pattern = @"(?></?\w +)(?> (?:[^> '""]+|'[^']*' | ""[^""] * "")*)> ";
            strHtml = Regex.Replace(strHtml, pattern,string.Empty);


            return strHtml;

        }
        */
    }
}
