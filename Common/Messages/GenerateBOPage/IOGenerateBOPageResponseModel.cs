using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;

namespace IOBootstrap.NET.Common;

public class IOGenerateBOPageResponseModel : IOResponseModel
{
    public string EntityName { get; set; }
    public string EntityDisplayName { get; set; }
    public string EntityItemName { get; set; }
    public IList<IOBOPageEntityModel> Properties { get; set; }
    public string ListEntityName { get; set; }
    public string ListEntityAPIPath { get; set; }
    public string ListEntityDisplayName { get; set; }
    public string UpdateEntityName { get; set; }
    public string UpdateEntityAPIPath { get; set; }
    public string UpdateEntityDisplayName { get; set; }
    public string DeleteEntityName { get; set; }
    public string DeleteEntityAPIPath { get; set; }
    public string DeleteEntityDisplayName { get; set; }
    public string CreateEntityName { get; set; }
    public string CreateEntityAPIPath { get; set; }
    public string CreateEntityDisplayName { get; set; }

    public IOGenerateBOPageResponseModel() : base()
    {
    }
}
