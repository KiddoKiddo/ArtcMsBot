using Microsoft.Azure.CosmosDB.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARTCIntranet_5min
{
    public class lineRunningEntity : TableEntity
    {
        public lineRunningEntity(string indicator, string row)
        {
            this.PartitionKey = indicator;
            this.RowKey = row;
        }

        public lineRunningEntity() { }
        public string Station { get; set; }
        public bool Result { get; set; }
    }

    public class stationEntity : TableEntity
    {
        public stationEntity(string station, string row)
        {
            this.PartitionKey = station;
            this.RowKey = row;
        }

        public stationEntity() { }
        public string Station { get; set; }
        public bool Result { get; set; }
    }
}
