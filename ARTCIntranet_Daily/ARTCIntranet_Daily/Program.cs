﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.Storage;
using Microsoft.Azure.CosmosDB.Table;
using Newtonsoft.Json;
using System.Threading;

namespace ARTCIntranet_Daily
{
    class Program
    {
        static void Main(string[] args)
        {
            string appKey = "";

            Console.WriteLine("This is a console app that publish data from Thingworx to Azure Cosmos DB...");
            Console.WriteLine("Please enter appKey...");
            appKey = Console.ReadLine();

            if (appKey == "36ffdfc0-5338-405b-8217-3f010f89e012")
            {
                Console.WriteLine("The key is verified. The process will start now.");
                List<string> indicatorList1 = new List<string>();
                List<string> indicatorList2 = new List<string>();

                indicatorList1.Add("CycleTime");
                indicatorList1.Add("WorkingTime");

                indicatorList2.Add("OEE");
                indicatorList2.Add("Productivity");
                indicatorList2.Add("QualityValue");
                indicatorList2.Add("TotalParts");
                indicatorList2.Add("GoodParts");

                runProcess(appKey, indicatorList1, indicatorList2);
            }
            else
            {
                Console.WriteLine("Sorry, this key is not authorized.");
            }

            Console.WriteLine("Press enter key to exit..."); 
            Console.ReadLine();
        }

        private static async void runProcess(string appKey, List<string>indicatorList1, List<string>indicatorList2)
        {
            while (true)
            {
                foreach (string indicator in indicatorList1)
                {
                    Process.Process1(appKey, indicator);
                    //Console.WriteLine(indicator + " is uploaded.");
                    await Task.Delay(500);
                }

                foreach (string indicator in indicatorList2)
                {
                    Process.Process2(appKey, indicator);
                    //Console.WriteLine(indicator + " is uploaded.");
                    await Task.Delay(500);
                }

                //delay for 24 hours
                await Task.Delay(TimeSpan.FromHours(24));
            }
        }
    }

    class Process
    {
        protected static Dictionary<string, string> urlDict = new Dictionary<string, string>
        {
            {
                "CycleTime",
                "GetCycleTimeForStationByDate"
            },
            {
                "WorkingTime",
                "GetWorkingTimeAtStationByDate"
            },
            {
                "OEE",
                "GetOEEByDate"
            },
            {
                "Productivity",
                "GetProductivityByDate"
            },
            {
                "QualityValue",
                "GetQualityValueByDate"
            },
            {
                "TotalParts",
                "GetTotalPartsByDate"
            },
            {
                "GoodParts",
                "GetGoodPartsByDate"
            }
        };

        private const string baseURL = "http://192.168.128.51:8080/Thingworx/Things/MSChatBot_API/Services/";

        public static async void Process1(string appKey, string indicator)
        {
            // this is for cycleTime and workingTime
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Add("appKey", appKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                List<string> stationNo = new List<string>();
                stationNo.Add("1");
                stationNo.Add("2");
                stationNo.Add("3");
                stationNo.Add("cobot");

                foreach (var station in stationNo)
                {
                    byte[] byteData = Encoding.UTF8.GetBytes("{\"station\": \"" + station + "\"}");
                    string url;
                    if (urlDict.TryGetValue(indicator, out url))
                    {
                        var response = await CallEndPoint(client, url, byteData);
                        sendToAzure(indicator, station, response.ToString());
                        Console.WriteLine($"{indicator} @ {station}: {response.ToString()}");
                    }
                }
            }
        }

        public static async void Process2(string appKey, string indicator)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Add("appKey", appKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                byte[] byteData = Encoding.UTF8.GetBytes("");
                string url;
                if (urlDict.TryGetValue(indicator, out url))
                {
                    var response = await CallEndPoint(client, url, byteData);
                    sendToAzure(indicator, indicator, response.ToString());
                    Console.WriteLine($"{indicator}: {response.ToString()}");
                }
            }
        }

        private static async Task<double> CallEndPoint(HttpClient client, string url, byte[] byteData)
        {
            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();
                Rootobject jsonObj = JsonConvert.DeserializeObject<Rootobject>(result);
                return jsonObj.rows[0].result;
            }
        }

        private static async void sendToAzure(string indicator, string station, string value)
        {
            string baseURL = "<Azure Function>";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                byte[] byteData = Encoding.UTF8.GetBytes("{\"indicator\": \"" + indicator + "\", \"station\": \"" + station + "\", \"value\":\"" + value + "\"}");
                var itemContent = new ByteArrayContent(byteData);
                itemContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync(baseURL, itemContent);
            }
        }
    }
}
