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

namespace ARTCIntranet_Hourly
{
    class Program
    {
        static void Main(string[] args)
        {
            string appKey = "";

            Console.WriteLine("This is a console app that publish data from Thingworx to Azure Cosmos DB...");
            Console.WriteLine("Please enter appKey...");
            appKey = Console.ReadLine();

            if(appKey == "36ffdfc0-5338-405b-8217-3f010f89e012")
            {
                Console.WriteLine("The key is verified. The process will start now.");
                runProcess(appKey);
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Sorry, this key is not authorized.");
            }

            Console.WriteLine("Press enter key to exit...");
            Console.ReadLine();
        }

        private static async void runProcess(string appKey)
        {
            while (true)
            {
                Process.defectProcess(appKey);
                //Console.WriteLine("latestDefect is uploaded.");
                Process.scrapValue(appKey);
                //Console.WriteLine("scrapValue is uploaded.");
                await Task.Delay(TimeSpan.FromHours(1));
            }
        }
    }

    class Process
    {
        private const string baseURL = "http://192.168.128.51:8080/Thingworx/Things/MSChatBot_API/Services/";

        public static async void defectProcess(string appKey)
        {
            //string
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Add("appKey", appKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string url = "GetLatestDefect";
                byte[] byteData = Encoding.UTF8.GetBytes("");
                var response = await CallEndPointString(client, url, byteData);
                sendToAzure("LatestDefect", "LatestDefect", response);
                Console.WriteLine($"LatestDefect: {response}");
            }
        }

        public static async void scrapValue(string appKey)
        {
            //double
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

                string url = "GetScrapValueAtStation";

                foreach (var station in stationNo)
                {
                    byte[] byteData = Encoding.UTF8.GetBytes("{\"station\": \"" + station + "\"}");
                    var response = await CallEndPointDouble(client, url, byteData);
                    sendToAzure("ScrapValue", station, response.ToString());
                    Console.WriteLine($"ScrapValue @ {station}: {response.ToString()}");
                }
            }
        }

        private static async Task<double> CallEndPointDouble(HttpClient client, string url, byte[] byteData)
        {
            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();
                RootobjectDouble jsonObj = JsonConvert.DeserializeObject<RootobjectDouble>(result);
                return jsonObj.rows[0].result;
            }
        }

        private static async Task<string> CallEndPointString(HttpClient client, string url, byte[] byteData)
        {
            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();
                RootobjectString jsonObj = JsonConvert.DeserializeObject<RootobjectString>(result);
                return jsonObj.rows[0].result;
            }
        }

        private static async void sendToAzure(string indicator, string station, string value)
        {
            string baseURL = "<Azure Function>";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                byte[] byteData = Encoding.UTF8.GetBytes("{\"indicator\": \"" + indicator + "\", \"station\": \"" + station + "\", \"value\":\"" + value.ToString() + "\"}");
                var itemContent = new ByteArrayContent(byteData);
                itemContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync(baseURL, itemContent);
            }
        }
    }
}
