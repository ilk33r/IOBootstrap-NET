using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common.Models.GenerateBOPage;

public class IOBOPageEntityModel : IOModel
{
    [IOBackofficeRequest]
    public string? PropertyName { get; set; }
    [IOBackofficeRequest]
    public string? PropertyJsonKey { get; set; }
    public IOBOPagePropertyType Type { get; set; }
    public bool Nullable { get; set; }
    public int? StringLength { get; set; }
    [IOBackofficeRequest]
    public string? EnumTypeName { get; set; }
    public IList<IOBOPageEntityCustomEnumTypeModel>? EnumType { get; set; }
}
