using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;


public class status5Object
{
    public Value5[] value { get; set; }
}

public class Value5
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTime Timestamp { get; set; }
    public string Value { get; set; }
}


public class status1Object
{
    public Value1[] value { get; set; }
}

public class Value1
{
    public DateTime Timestamp { get; set; }
    public string Indicator { get; set; }
    public string Station { get; set; }
    public string Value { get; set; }
}



