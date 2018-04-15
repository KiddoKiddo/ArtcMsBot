using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Collections.Generic;
using BotAuth.Models;
using System.Configuration;
using BotAuth.Dialogs;
using BotAuth.AADv2;
using System.Net.Http;
using System.Threading;
using BotAuth;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.Azure.CosmosDB.Table;

namespace ARTC_PilotBot.Dialogs
{
    [LuisModel("5e04e521-7121-4cd3-82fc-5c54317de9a9", "771640b3f28f454a961892b61102f428")]
    [Serializable]
    public class MainDialog : LuisDialog<object>
    {
        protected List<string> memberGroup = new List<string>();

        [LuisIntent("loginService")]
        public async Task loginService(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            //prompt authentication and retrieve groups
            await getUserMemberGroup(context);
        }

        [LuisIntent("None")]
        public async Task None(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            //prompt authentication and retrieve groups
            //await getUserMemberGroup(context,"IT", "None");
            await context.PostAsync("Sorry, I don't understand what you are saying.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("askStatus")]
        public async Task askStatus(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            //prompt authentication and retrieve groups
            //await getUserMemberGroup(context, "IT", "askStatus");
            bool status = checkAuthority(memberGroup, "IT");
            
            if(status)
            {
                //await luisResponse(context, result);
                string replyText = await getStatus.getStatusFromAzure("oee");
                await context.PostAsync(replyText);
                context.Wait(MessageReceived);
            }
            else
            {
                await context.PostAsync("Sorry, you are not authorized, please sign in with an authorized account.");
                context.Wait(MessageReceived);
            }
        }

        [LuisIntent("defectDetection")]
        public async Task defectDetection(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            //prompt authentication and retrieve groups
            //await getUserMemberGroup(context, "IT", "defectDetection");
            bool status = checkAuthority(memberGroup, "IT");

            if (status)
            {
                await luisResponse(context, result);
            }
            else
            {
                await context.PostAsync("Sorry, you are not authorized, please sign in with an authorized account.");
                context.Wait(MessageReceived);
            }
        }

        [LuisIntent("factoryStatus")]
        public async Task factoryStatus(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            //prompt authentication and retrieve groups
            //await getUserMemberGroup(context,"IT", "factoryStatus");
            bool status = checkAuthority(memberGroup, "IT");

            if (status)
            {
                await luisResponse(context, result);
            }
            else
            {
                await context.PostAsync("Sorry, you are not authorized, please sign in with an authorized account.");
                context.Wait(MessageReceived);
            }
        }

        [LuisIntent("operatorAvailability")]
        public async Task operatorAvailability(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            //prompt authentication and retrieve groups
            //await getUserMemberGroup(context,"IT", "operatorAvailability");
            bool status = checkAuthority(memberGroup, "IT");

            if (status)
            {
                //await luisResponse(context, result);
                await context.PostAsync("There's no operator right now.");
                context.Wait(MessageReceived);
            }
            else
            {
                await context.PostAsync("Sorry, you are not authorized, please sign in with an authorized account.");
                context.Wait(MessageReceived);
            }
        }

        [LuisIntent("operatorTime")]
        public async Task operatorTime(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            //prompt authentication and retrieve groups
            //await getUserMemberGroup(context,"IT", "operatorTime");
            bool status = checkAuthority(memberGroup, "IT");

            if (status)
            {
                await luisResponse(context, result);
            }
            else
            {
                await context.PostAsync("Sorry, you are not authorized, please sign in with an authorized account.");
                context.Wait(MessageReceived);
            }
        }

        [LuisIntent("productionYield")]
        public async Task productionYield(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            //prompt authentication and retrieve groups
            //await getUserMemberGroup(context, "IT", "productionYield");
            bool status = checkAuthority(memberGroup, "IT");

            if (status)
            {
                await luisResponse(context, result);
            }
            else
            {
                await context.PostAsync("Sorry, you are not authorized, please sign in with an authorized account.");
                context.Wait(MessageReceived);
            }
        }

        private async Task luisResponse(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("This is what I detected:");
            string intent = result.Intents[0].Intent;
            await context.PostAsync($"Top scoring intent: {intent}");
            string entitiesType = "";
            string entitiesName = "";
            int entitiesCount = result.Entities.Count;
            for(int i = 0; i < entitiesCount; i++)
            {
                entitiesType = result.Entities[i].Type;
                entitiesName = result.Entities[i].Entity;
                await context.PostAsync($"{entitiesType}: {entitiesName}");
            }
        }

        private async Task getUserMemberGroup(IDialogContext context)
        {
            AuthenticationOptions options = new AuthenticationOptions()
            {
                Authority = ConfigurationManager.AppSettings["aad:Authority"],
                ClientId = ConfigurationManager.AppSettings["aad:ClientId"],
                ClientSecret = ConfigurationManager.AppSettings["aad:ClientSecret"],
                Scopes = new string[] { "Directory.Read.All" },
                RedirectUrl = ConfigurationManager.AppSettings["aad:Callback"]
            };

            await context.Forward(new AuthDialog(new MSALAuthProvider(), options), async (IDialogContext authContext, IAwaitable<AuthResult> authResult) =>
            {
                var result2 = await authResult;
                var json = await new HttpClient().GetWithAuthAsync(result2.AccessToken, "https://graph.microsoft.com/v1.0/me/memberOf");
                string jsonString = json.ToString(Formatting.None);
                userMemberObj memberJSON = JsonConvert.DeserializeObject<userMemberObj>(jsonString);

                int memberCount = memberJSON.value.Count();

                for(int i = 0; i < memberCount; i++)
                {
                    this.memberGroup.Add(memberJSON.value[i].displayName);
                }

                await authContext.PostAsync("You are authorized to access the information now.");
            }, context.Activity, CancellationToken.None);
        }

        private bool checkAuthority(List<string>memberGroup, string role)
        {
            if(memberGroup.Contains(role))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}