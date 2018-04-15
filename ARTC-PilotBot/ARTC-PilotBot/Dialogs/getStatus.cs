using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace ARTC_PilotBot.Dialogs
{
    public class getStatus
    {
        private const string baseURL = "https://artcmsbotfuncbotb36b.table.core.windows.net/OEE?st=2018-03-25T09%3A32%3A00Z&se=2022-03-27T09%3A32%3A00Z&sp=rau&sv=2017-04-17&tn=oee&sig=";
        private const string tableSig = "4Qejlyjouky8njQTgJgeRHVHUw9T4byUc5YUxgh8q%2B8%3D";

        public static async Task<string> getStatusFromAzure(string indicator)
        {
            indicator = indicator.ToUpper();
            var http = new HttpClient();
            http.DefaultRequestHeaders.Add("Accept", "application/json;odata=nometadata");
            string checkFilter = "&$filter=Indicator%20eq%20'" + indicator + "'";
            string requestURL = baseURL + tableSig + checkFilter;
            var response = await http.GetAsync(requestURL);
            var result2 = await response.Content.ReadAsStringAsync();
            oeeObj jsonOBJ = JsonConvert.DeserializeObject<oeeObj>(result2);
            string replyText = $"Current {indicator} reading is " + jsonOBJ.value[0].Reading.ToString() + ".";
            return replyText;
        }

    }
}