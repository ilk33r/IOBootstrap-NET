using IOBootstrap.NET.Common.Models.Base;

namespace IOBootstrap.NET.Common;

public class IOBOPageEntityModel : IOModel
{
    public string PropertyName { get; set; }
    public string PropertyJsonKey { get; set; }
    public IOBOPagePropertyType Type { get; set; }
    public bool Nullable { get; set; }
    public int? StringLength { get; set; }
    public string EnumTypeName { get; set; }
    public IList<IOBOPageEntityCustomEnumTypeModel> EnumType { get; set; }
}
