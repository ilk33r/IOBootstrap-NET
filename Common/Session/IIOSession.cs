namespace IOBootstrap.NET.Common.Session;

public interface IIOSession
{

    string SessionID { get; }
    
    public string? Get(string key);
    public void Set(string key, string value);
    public void Flush();
}
