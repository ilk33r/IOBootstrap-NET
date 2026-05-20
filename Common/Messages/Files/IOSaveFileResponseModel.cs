using System;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common.Messages.Files;

public class IOSaveFileResponseModel : IOResponseModel
{
    public IOFileVariationsModel File { get; set; }

    public IOSaveFileResponseModel(IOFileVariationsModel file) : base()
    {
        File = file;
    }
}
