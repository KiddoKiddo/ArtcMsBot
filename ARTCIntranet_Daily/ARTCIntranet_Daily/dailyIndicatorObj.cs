using Microsoft.Azure.CosmosDB.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARTCIntranet_Daily
{
    public class cycleTimeEntity : TableEntity
    {
        public cycleTimeEntity(string indicator, string station)
        {
            this.PartitionKey = indicator;
            this.RowKey = station;
        }

        public cycleTimeEntity() { }
        public string Station { get; set; }
        public double Result { get; set; }
    }

    public class workingTimeEntity : TableEntity
    {
        public workingTimeEntity(string indicator, string station)
        {
            this.PartitionKey = indicator;
            this.RowKey = station;
        }

        public workingTimeEntity() { }
        public string Station { get; set; }
        public double Result { get; set; }
    }

    public class oeeEntity : TableEntity
    {
        public oeeEntity(string indicator, string row)
        {
            this.PartitionKey = indicator;
            this.RowKey = row;
        }

        public oeeEntity() { }
        public string Station { get; set; }
        public double Result { get; set; }
    }

    public class productivityEntity : TableEntity
    {
        public productivityEntity(string indicator, string row)
        {
            this.PartitionKey = indicator;
            this.RowKey = row;
        }

        public productivityEntity() { }
        public string Station { get; set; }
        public double Result { get; set; }
    }

    public class qualityValueEntity : TableEntity
    {
        public qualityValueEntity(string indicator,string row)
        {
            this.PartitionKey = indicator;
            this.RowKey = row;
        }

        public qualityValueEntity() { }
        public string Station { get; set; }
        public double Result { get; set; }
    }

    public class totalPartsEntity : TableEntity
    {
        public totalPartsEntity(string indicator, string row)
        {
            this.PartitionKey = indicator;
            this.RowKey = row;
        }

        public totalPartsEntity() { }
        public string Station { get; set; }
        public double Result { get; set; }
    }

    public class goodPartsEntity : TableEntity
    {
        public goodPartsEntity(string indicator, string row)
        {
            this.PartitionKey = indicator;
            this.RowKey = row;
        }

        public goodPartsEntity() { }
        public string Station { get; set; }
        public double Result { get; set; }
    }
}
