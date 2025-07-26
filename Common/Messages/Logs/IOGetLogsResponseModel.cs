using System;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Logs;

public class IOGetLogsResponseModel : IOResponseModel
{
    public int Count { get; set; }
    public IList<IOLogModel> Logs { get; set; }

    public IOGetLogsResponseModel(int count, IList<IOLogModel> logs) : base()
    {
        Count = count;
        Logs = logs;
    }
}
