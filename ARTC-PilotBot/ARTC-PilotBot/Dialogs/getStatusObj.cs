using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;


public class oeeObj
{
    public Value2[] value { get; set; }
}

public class Value2
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTime Timestamp { get; set; }
    public string Indicator { get; set; }
    public float Reading { get; set; }
}


