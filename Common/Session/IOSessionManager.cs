using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Utilities;

namespace IOBootstrap.NET.Common.Session;

public class IOSessionManager : IIOSession
{
    public string SessionID { get; }

    private string SessionIDCacheKey { get; set; }
    private Dictionary<string, string> SessionData { get; set; }
    private int TokenLife { get; set; }
    
    public IOSessionManager(HttpRequest request, IConfiguration configuration)
    {
        TokenLife = configuration.GetValue<int>(IOConfigurationConstants.TokenLife)!;

        if (request.Headers.ContainsKey(IORequestHeaderConstants.SessionID))
        {
            SessionID = request.Headers[IORequestHeaderConstants.SessionID]!;
        }
        else
        {
            SessionID = IORandomUtilities.GenerateGUIDString();
        }

        SessionIDCacheKey = String.Format(IOCacheKeys.SessionCacheKey, SessionID);
        
        IOCacheObject? sessionDataCacheObject = IOCache.GetCachedObject(SessionIDCacheKey);
        if (sessionDataCacheObject == null)
        {
            SessionData = new Dictionary<string, string>();
        }
        else
        {
            SessionData = (Dictionary<string, string>)sessionDataCacheObject.Value;
        }
    }

    public string? Get(string key)
    {
        if (SessionData.ContainsKey(key))
        {
            return SessionData[key];
        }

        return null;
    }

    public void Set(string key, string value)
    {
        SessionData[key] = value;
    }

    public void Flush()
    {
        IOCacheObject sessionDataCacheObject = new IOCacheObject(SessionIDCacheKey, SessionData, TokenLife);
        IOCache.CacheObject(sessionDataCacheObject);
    }
}
