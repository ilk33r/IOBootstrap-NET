using System;
using System.Net;

namespace IOBootstrap.NET.Common.Utilities;

public static class IOHttpUtilities
{
    #region HTTP Helpers

    public static string? GetUserIP(HttpRequest request)
    {
        // Obtain ip list from forwaded
        string? ipList = request.Headers["HTTP_X_FORWARDED_FOR"];

        // Check ip list is not null
        if (!string.IsNullOrEmpty(ipList))
        {
            return ipList.Split(',')[0];
        }

        // Returrn ip address
        return request.HttpContext?.Connection?.RemoteIpAddress?.ToString();
    }

    public static string? GetUserHostName(HttpRequest request)
    {
        // Obtain ip list from forwaded
        string? ipList = request.Headers["HTTP_X_FORWARDED_FOR"];

        // Check ip list is not null
        if (!string.IsNullOrEmpty(ipList))
        {
            return ipList.Split(',')[0];
        }

        // Returrn ip address
        IPAddress? remoteIpAddress = request.HttpContext?.Connection?.RemoteIpAddress;
        if (remoteIpAddress != null)
        {
            return Dns.GetHostEntry(remoteIpAddress).HostName;
        }
        
        return null;
    }

    #endregion
}
