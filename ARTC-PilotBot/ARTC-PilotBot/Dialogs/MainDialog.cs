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

        [LuisIntent("loginService")] //Done
        public async Task loginService(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            //prompt authentication and retrieve groups
            await getUserMemberGroup(context);
        }

        [LuisIntent("None")] //Done
        public async Task None(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await context.PostAsync("Sorry, I don't understand what you are saying.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("askStatus")]
        public async Task askStatus(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            bool status = checkAuthority(memberGroup, "IT");
            
            if(status)
            {
                List<string> EntityType = new List<string>();
                List<string> EntityName = new List<string>();
                // check entities
                int entitiesCount = result.Entities.Count;
                for (int i = 0; i < entitiesCount; i++)
                {
                    EntityType.Add(result.Entities[i].Type);
                    EntityName.Add(result.Entities[i].Entity);
                }
                if(EntityType.Contains("Indicator"))
                {
                    string indicator = EntityName[EntityType.IndexOf("Indicator")].ToLower();
                    switch (indicator)
                    {
                        case ("scrap"):
                            if(EntityType.Contains("venue"))
                            {
                                string station = EntityName[EntityType.IndexOf("venue")];
                                string stationCall = station;
                                if(station.Contains("station"))
                                {
                                    stationCall = station.Substring(station.Length - 1);
                                }
                                status1Object JSON = await getStatus.getStatus1("ScrapValue", stationCall);
                                // set default
                                string calculation = "total";
                                DateTime duration = DateTime.UtcNow;
                                
                                if(EntityType.Contains("math"))
                                {
                                    calculation = EntityName[EntityType.IndexOf("math")];
                                }
                                if(EntityType.Contains("builtin.datetimeV2.date"))
                                {
                                    var resolutionValue = (IList<object>)result.Entities[0].Resolution["values"];
                                    foreach (var value in resolutionValue)
                                    {
                                        duration = Convert.ToDateTime(((IDictionary<string, object>)value)["value"]);
                                    }
                                }
                                string response = transformResponse.processScrapValue(JSON, calculation, duration);
                                await context.PostAsync(response);
                            }
                            else
                            {
                                await context.PostAsync("Are you missing venue?");
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    await context.PostAsync("Sorry, are you missing indicator?");
                }
                context.Wait(MessageReceived);
            }
            else
            {
                await context.PostAsync("Sorry, you are not authorized, please sign in with an authorized account.");
                context.Wait(MessageReceived);
            }
        }

        [LuisIntent("defectDetection")] //Done
        public async Task defectDetection(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            bool status = checkAuthority(memberGroup, "IT");

            if (status)
            {
                List<string> EntityType = new List<string>();
                List<string> EntityName = new List<string>();
                // check entities
                int entitiesCount = result.Entities.Count;
                for (int i = 0; i < entitiesCount; i++)
                {
                    EntityType.Add(result.Entities[i].Type);
                    EntityName.Add(result.Entities[i].Entity);
                }
                if(EntityType.Contains("venue"))
                {
                    string station = EntityName[EntityType.IndexOf("venue")];
                    string stationCall = station;
                    //trim station name
                    if (station.Contains("station"))
                    {
                        stationCall = station.Substring(station.Length - 1);
                    }
                    status1Object JSON = await getStatus.getStatus1("LatestDefect", ""); //check
                    string response = transformResponse.processDefect(JSON, station);
                    await context.PostAsync(response);
                }
                else
                {
                    await context.PostAsync("Sorry, are you missing the station?");
                }
                context.Wait(MessageReceived);
            }
            else
            {
                await context.PostAsync("Sorry, you are not authorized, please sign in with an authorized account.");
                context.Wait(MessageReceived);
            }
        }

        [LuisIntent("factoryStatus")] //Done
        public async Task factoryStatus(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            bool status = checkAuthority(memberGroup, "IT");

            if (status)
            {
                List<string> EntityType = new List<string>();
                List<string> EntityName = new List<string>();
                // check entities
                int entitiesCount = result.Entities.Count;
                for(int i = 0; i < entitiesCount; i++)
                {
                    EntityType.Add(result.Entities[i].Type);
                    EntityName.Add(result.Entities[i].Entity);
                }

                if(entitiesCount < 1)
                {
                    await context.PostAsync("Sorry, I can't answer you. Are you missing something?");
                }
                else
                {
                    if(EntityName.Contains("inlet") || EntityName.Contains("outlet"))
                    {
                        string station = EntityName[EntityType.IndexOf("venue")];
                        string response = await getStatus.getStatus5("IsTrolleyDocked", station);
                        bool output = Boolean.Parse(response);
                        if(output)
                        {
                            await context.PostAsync($"{station} is docked.");
                        }
                        else
                        {
                            await context.PostAsync($"{station} is not docked.");
                        }
                    }
                    else
                    {
                        string response = await getStatus.getStatus5("IsLineRunning", "");
                        bool output = Boolean.Parse(response);
                        if(output)
                        {
                            await context.PostAsync("Yes, it is running.");
                        }
                        else
                        {
                            await context.PostAsync("No, it's not running.");
                        }
                    }
                }
                context.Wait(MessageReceived);
            }
            else
            {
                await context.PostAsync("Sorry, you are not authorized, please sign in with an authorized account.");
                context.Wait(MessageReceived);
            }
        }

        [LuisIntent("operatorAvailability")] //Done
        public async Task operatorAvailability(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            bool status = checkAuthority(memberGroup, "IT");

            if (status)
            {
                List<string> EntityType = new List<string>();
                List<string> EntityName = new List<string>();
                // check entities
                int entitiesCount = result.Entities.Count;
                for (int i = 0; i < entitiesCount; i++)
                {
                    EntityType.Add(result.Entities[i].Type);
                    EntityName.Add(result.Entities[i].Entity);
                }

                if(EntityType.Contains("venue"))
                {
                    string station = EntityName[EntityType.IndexOf("venue")];
                    string stationCall = station;
                    //trim station name
                    if(station.Contains("station"))
                    {
                        stationCall = station.Substring(station.Length - 1);
                    }
                    string response = await getStatus.getStatus5("IsWorkerAtStation", stationCall);               
                    bool output = Boolean.Parse(response);
                    if(output)
                    {
                        await context.PostAsync($"There's worker in {station}");
                    }
                    else
                    {
                        await context.PostAsync($"There's no worker in {station}");
                    }
                }
                else
                {
                    await context.PostAsync("Sorry, are you missing the venue?");
                }
                context.Wait(MessageReceived);
            }
            else
            {
                await context.PostAsync("Sorry, you are not authorized, please sign in with an authorized account.");
                context.Wait(MessageReceived);
            }
        }

        [LuisIntent("operatorTime")] //Done
        public async Task operatorTime(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            bool status = checkAuthority(memberGroup, "IT");

            if (status)
            {
                List<string> EntityType = new List<string>();
                List<string> EntityName = new List<string>();
                // check entities
                int entitiesCount = result.Entities.Count;
                for (int i = 0; i < entitiesCount; i++)
                {
                    EntityType.Add(result.Entities[i].Type);
                    EntityName.Add(result.Entities[i].Entity);
                }
                if(EntityType.Contains("venue"))
                {
                    string station = EntityName[EntityType.IndexOf("venue")];
                    string stationCall = station;
                    DateTime duration = DateTime.UtcNow;
                    //trim station name
                    if (station.Contains("station"))
                    {
                        stationCall = station.Substring(station.Length - 1);
                    }                 
                    if (EntityType.Contains("builtin.datetimeV2.date"))
                    {
                        var resolutionValue = (IList<object>)result.Entities[0].Resolution["values"];
                        foreach (var value in resolutionValue)
                        {
                            duration = Convert.ToDateTime(((IDictionary<string, object>)value)["value"]);
                        }
                    }
                    status1Object JSON = await getStatus.getStatus1("workingTime", stationCall);
                    string response = transformResponse.processWorkingTime(JSON, duration, station);
                    await context.PostAsync(response);
                }
                else
                {
                    await context.PostAsync("Are you missing the station?");
                }
                context.Wait(MessageReceived);
            }
            else
            {
                await context.PostAsync("Sorry, you are not authorized, please sign in with an authorized account.");
                context.Wait(MessageReceived);
            }
        }

        [LuisIntent("productionYield")] //Done
        public async Task productionYield(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            bool status = checkAuthority(memberGroup, "IT");

            if (status)
            {
                List<string> EntityType = new List<string>();
                List<string> EntityName = new List<string>();
                // check entities
                int entitiesCount = result.Entities.Count;
                for (int i = 0; i < entitiesCount; i++)
                {
                    EntityType.Add(result.Entities[i].Type);
                    EntityName.Add(result.Entities[i].Entity);
                }
                if(EntityType.Contains("yield.option"))
                {
                    string option = EntityName[EntityType.IndexOf("yield.option")].ToLower();
                    DateTime duration = DateTime.UtcNow;
                    if (EntityType.Contains("builtin.datetimeV2.date"))
                    {
                        var resolutionValue = (IList<object>)result.Entities[0].Resolution["values"];
                        foreach (var value in resolutionValue)
                        {
                            duration = Convert.ToDateTime(((IDictionary<string, object>)value)["value"]);
                        }
                    }
                    if (option == "good")
                    {
                        status1Object JSON = await getStatus.getStatus1("GoodParts", "");
                        string response = transformResponse.processTotalAndGoodParts(JSON, duration, "Good parts");
                        await context.PostAsync(response);
                    }
                    else
                    {
                        status1Object JSON = await getStatus.getStatus1("TotalParts", "");
                        string response = transformResponse.processTotalAndGoodParts(JSON, duration, "Total parts");
                        await context.PostAsync(response);
                    }
                }
                else
                {
                    await context.PostAsync("Are you missing something?");
                }
                context.Wait(MessageReceived);
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