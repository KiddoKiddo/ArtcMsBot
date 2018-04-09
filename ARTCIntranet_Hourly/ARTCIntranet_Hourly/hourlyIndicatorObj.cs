using Microsoft.Azure.CosmosDB.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARTCIntranet_Hourly
{
    public class defectEntity : TableEntity
    {
        public defectEntity(string indicator, string row)
        {
            this.PartitionKey = indicator;
            this.RowKey = row;
        }

        public defectEntity() { }
        public string Station { get; set; }
        public string Result { get; set; }
    }

    public class scrapEntity : TableEntity
    {
        public scrapEntity(string indicator, string station)
        {
            this.PartitionKey = indicator;
            this.RowKey = station;
        }

        public scrapEntity() { }
        public string Station { get; set; }
        public double Result { get; set; }
    }
}
