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
                        await turnContext.SendActivityAsync(MessageFactory.Text("�ȳ��ϼ���! {d}"), cancellationToken);
                        break;
                    case "Boring":
                        switch (ran.Next(0, 2))
                        {
                            case 0:
                                await turnContext.SendActivityAsync(MessageFactory.Text("��մ� ����̶� �ұ��? �Ƹ�尡 ������ ���Կ�? ���̾Ƹ��! �����?{a}"), cancellationToken);
                                break;
                            default:
                                await turnContext.SendActivityAsync(MessageFactory.Text("���� ���� ��ƿ�! {a}"), cancellationToken);
                                break;
                        }
                        break;
                    case "Compliment":
                        await turnContext.SendActivityAsync(MessageFactory.Text("Ī�� �����մϴ�! {a}"), cancellationToken);
                        break;
                    case "Bye":
                        await turnContext.SendActivityAsync(MessageFactory.Text("���̹���! {0}"), cancellationToken);
                        break;
                    case "Credit":
                        await turnContext.SendActivityAsync(MessageFactory.Text("�� ���� �е��� �� KUHT�� ���α�,��Ժ�,������,���¿����Դϴ�! {a}"), cancellationToken);
                        break;
                    case "Sadness":
                        switch (ran.Next(0,3))
                        {
                            case 0:
                                await turnContext.SendActivityAsync(MessageFactory.Text("����..���� ������ ���Ŀ�.. {b}"), cancellationToken);
                                break;
                            case 1:
                                await turnContext.SendActivityAsync(MessageFactory.Text("���� ���°���? ���ߵ������ �� �����ƿ�.{b}"), cancellationToken);
                                break;
                            default:
                                await turnContext.SendActivityAsync(MessageFactory.Text("�����ƿ�? �� ����� ������ �ʾ������ؿ�..{b}"), cancellationToken);
                                break;
                        }
                        break;
                    case "blame":
                        await turnContext.SendActivityAsync(MessageFactory.Text("�����, ���� �ƹ��� �κ��̶����� ���� �ʹ� ���Ѱ� �ƴϿ���? {c}"), cancellationToken);
                        break;
                    case "Web.WebSearch":
                        string search_word = applyJObj["prediction"]["entities"]["Web.SearchText"][0].ToString();
                        await turnContext.SendActivityAsync(MessageFactory.Text(GetSearch(search_word)+"{0}"), cancellationToken);
                        break;
                    default:
                        await turnContext.SendActivityAsync(MessageFactory.Text("��..�� �𸣰ھ��! {0}"), cancellationToken);
                        break;


                }
                
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            
            var welcomeText = "��Ƽ, �⵿�Ͽ����ϴ�.";
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
            string url = "https://openapi.naver.com/v1/search/encyc?query=" + query; // ����� JSON ����
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("X-Naver-Client-Id", "StRrATopAv7DypIZ1D6s"); // Ŭ���̾�Ʈ ���̵�
            request.Headers.Add("X-Naver-Client-Secret", "eFD94njxCz");       // Ŭ���̾�Ʈ ��ũ��
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
                return "@����@";

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
