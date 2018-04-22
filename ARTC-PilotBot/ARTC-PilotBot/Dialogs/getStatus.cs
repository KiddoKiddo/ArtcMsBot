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
        private const string baseURL5 = "https://artctestdemobcab.table.core.windows.net/update5min?st=2018-04-21T16%3A46%3A00Z&se=2020-04-23T16%3A46%3A00Z&sp=r&sv=2017-04-17&tn=update5min&sig=";
        private const string tableSig5 = "O%2Bwyi7AFbyke6s0xuO4K1eXXtyP77P7ZcqawOD3qtac%3D";

        private const string baseURL1 = "https://artctestdemobcab.table.core.windows.net/indicator?st=2018-04-21T00%3A46%3A00Z&se=2020-04-24T00%3A46%3A00Z&sp=r&sv=2017-04-17&tn=indicator&sig=";
        private const string tableSig1 = "d1wRbNYVT5o%2BjmMbuCjNJkaTY1sLgj0viU483568Hag%3D";

        public static async Task<string> getStatus5(string indicator, string station)
        {
            var http = new HttpClient();
            http.DefaultRequestHeaders.Add("Accept", "application/json;odata=nometadata");
            string checkFilter = "&$filter=PartitionKey%20eq%20'" + indicator + "'";
            if(station != "")
            {
                checkFilter = checkFilter + "%20and%20RowKey%20eq%20'" + station + "'";
            }
            string requestURL = baseURL5 + tableSig5 + checkFilter;
            var response = await http.GetAsync(requestURL);
            var result2 = await response.Content.ReadAsStringAsync();
            status5Object JSONObj = JsonConvert.DeserializeObject<status5Object>(result2);
            return JSONObj.value[0].Value;
        }

        public static async Task<status1Object> getStatus1(string indicator, string station)
        {
            var http = new HttpClient();
            http.DefaultRequestHeaders.Add("Accept", "application/json;odata=nometadata");
            string columnSelection = "&$select=Timestamp,Indicator,Station,Value";

            string checkFilter = "&$filter=Indicator%20eq%20'" + indicator + "'";

            if (station != "")
            {
                checkFilter = checkFilter + "%20and%20Station%20eq%20'" + station + "'";
            }

            string requestURL = baseURL1 + tableSig1 + columnSelection + checkFilter;
            var response = await http.GetAsync(requestURL);
            var result2 = await response.Content.ReadAsStringAsync();
            status1Object JSONObj = JsonConvert.DeserializeObject<status1Object>(result2);
            return JSONObj;
        }

    }
}