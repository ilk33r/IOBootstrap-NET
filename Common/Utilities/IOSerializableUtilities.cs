using System.Text.Json;

namespace IOBootstrap.NET.Common.Utilities;

public static class IOSerializableUtilities
{
    public static TObject Copy<TObject>(TObject source)
    {
        string jsonValue = JsonSerializer.Serialize(source);
        TObject jsonObject = JsonSerializer.Deserialize<TObject>(jsonValue);
        return jsonObject;
    }
}