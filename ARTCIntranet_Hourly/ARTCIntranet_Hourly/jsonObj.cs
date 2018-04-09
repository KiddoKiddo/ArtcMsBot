using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RootobjectDouble
{
    public Datashape dataShape { get; set; }
    public Row[] rows { get; set; }
}

public class Datashape
{
    public Fielddefinitions fieldDefinitions { get; set; }
}

public class Fielddefinitions
{
    public Result result { get; set; }
}

public class Result
{
    public string name { get; set; }
    public string description { get; set; }
    public string baseType { get; set; }
    public int ordinal { get; set; }
    public Aspects aspects { get; set; }
}

public class Aspects
{
}

public class Row
{
    public double result { get; set; }
}

