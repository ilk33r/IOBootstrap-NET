﻿using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common;

public class IOGenerateBOPageResponseModel : IOResponseModel
{
    public string EntityName { get; set; }
    public IList<IOBOPageEntityModel> Properties { get; set; }
    public string ListEntityName { get; set; }
    public string ListEntityAPIPath { get; set; }
    public string ListEntityAction { get; set; }
    public string ListEntityDisplayName { get; set; }

    public IOGenerateBOPageResponseModel() : base()
    {
    }
}
