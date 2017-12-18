namespace GraphDB.Contract.Enum
{
    //系统错误码
    public enum ErrorCode
    {
        NoError = 0,
        FileNotExists = 1,
        OpenFileFailed = 2,
        SaveFileFailed = 3,
        NoXmlRoot = 4,
        InvaildIndex = 10,
        NodeExists = 11,
        CreateNodeFailed = 12,
        NodeNotExists = 13,
        EdgeExists = 15,
        CreateEdgeFailed = 16,
        EdgeNotExists = 17,
        AddEdgeFailed = 18,
        InvalidParameter = 21,
        CypherInvalid = 40,
        StartSegInvalid = 41,
        MatchSegInvalid = 42,
        WhereSegInvalid = 43,
        ReturnSegInvalid = 44,
        NoStartNode = 60,
    }
}
