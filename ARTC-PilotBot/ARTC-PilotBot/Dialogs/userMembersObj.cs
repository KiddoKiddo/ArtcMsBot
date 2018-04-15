using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class userMemberObj
{
    public string odatacontext { get; set; }
    public string odatanextLink { get; set; }
    public Value[] value { get; set; }
}

public class Value
{
    public string odatatype { get; set; }
    public string id { get; set; }
    public object deletedDateTime { get; set; }
    public string classification { get; set; }
    public DateTime createdDateTime { get; set; }
    public string description { get; set; }
    public string displayName { get; set; }
    public string[] groupTypes { get; set; }
    public string mail { get; set; }
    public bool mailEnabled { get; set; }
    public string mailNickname { get; set; }
    public DateTime? onPremisesLastSyncDateTime { get; set; }
    public object[] onPremisesProvisioningErrors { get; set; }
    public string onPremisesSecurityIdentifier { get; set; }
    public bool? onPremisesSyncEnabled { get; set; }
    public object preferredDataLocation { get; set; }
    public string[] proxyAddresses { get; set; }
    public DateTime renewedDateTime { get; set; }
    public bool securityEnabled { get; set; }
    public string visibility { get; set; }
}
