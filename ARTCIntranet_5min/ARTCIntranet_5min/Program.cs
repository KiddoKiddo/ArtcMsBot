using Microsoft.Azure;
using Microsoft.Azure.CosmosDB.Table;
using Microsoft.Azure.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ARTCIntranet_5min
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
                runProcess(appKey);
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Sorry, this key is not authorized.");
                Console.WriteLine("Press enter key to exit...");
                Console.ReadLine();
            }           
        }

        private static async void runProcess(string appKey)
        {
            while(true)
            {
                Process.lineRunning(appKey);
                //Console.WriteLine("lineRunning is uploaded.");
                Process.trolleyDocked(appKey);
                //Console.WriteLine("trolleyDocked is uploaded.");
                Process.workerStation(appKey);
                //Console.WriteLine("workerStation is uploaded.");
                await Task.Delay(TimeSpan.FromMinutes(5));
            }
        }
    }

    class Process
    {
        private const string baseURL = "http://192.168.128.51:8080/Thingworx/Things/MSChatBot_API/Services/";

        public static async void lineRunning(string appKey)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Add("appKey", appKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                byte[] byteData = Encoding.UTF8.GetBytes("");
                string url = "IsLineRunning";
                var response = await CallEndPoint(client, url, byteData);
                sendToAzure("IsLineRunning", "IsLineRunning", response.ToString());
                Console.WriteLine($"IsLineRunning: {response.ToString()}");
            }
        }

        public static async void trolleyDocked(string appKey)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Add("appKey", appKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                List<string> zoneNo = new List<string>();
                zoneNo.Add("inlet");
                zoneNo.Add("outlet");

                foreach (var zone in zoneNo)
                {
                    byte[] byteData = Encoding.UTF8.GetBytes("{\"zone\": \"" + zone + "\"}");
                    string url = "IsTrolleyDocked";
                    var response = await CallEndPoint(client, url, byteData);
                    sendToAzure("IsTrolleyDocked", zone, response.ToString());
                    Console.WriteLine($"IsTrolleyDocked @ {zone}: {response.ToString()}");
                }
            }

        }

        public static async void workerStation(string appKey)
        {

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
                    string url = "IsWorkerAtStation";
                    var response = await CallEndPoint(client, url, byteData);
                    sendToAzure("IsWorkerAtStation", station, response.ToString());
                    Console.WriteLine($"IsWorkerAtStation @ {station}: {response.ToString()}");
                }
            }
        }

        private static async Task<bool> CallEndPoint(HttpClient client, string url, byte[] byteData)
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
                byte[] byteData = Encoding.UTF8.GetBytes("{\"partitionKey\": \"" + indicator + "\", \"rowKey\": \"" + station + "\", \"value\":\"" + value + "\"}");
                var itemContent = new ByteArrayContent(byteData);
                itemContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PutAsync(baseURL, itemContent);
            }
        }
    }
}
