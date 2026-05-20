using System;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Files;

public class IOGetFilesResponseModel : IOResponseModel
{
    public int Count { get; set; }
    public IList<IOFileVariationsModel> Files { get; set; }

    public IOGetFilesResponseModel(int count, IList<IOFileVariationsModel> files) : base()
    {
        Count = count;
        Files = files;
    }
}
