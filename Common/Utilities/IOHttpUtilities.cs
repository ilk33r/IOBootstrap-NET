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
            string splittedIPAddress = ipList.Split(',')[0];
            return splittedIPAddress.Split(':')[0];
        }

        // Returrn ip address
        return request.HttpContext?.Connection?.RemoteIpAddress?.ToString();
    }

    public static int? GetUserPort(HttpRequest request)
    {
        // Obtain ip list from forwaded
        string? ipList = request.Headers["HTTP_X_FORWARDED_FOR"];

        // Check ip list is not null
        if (!string.IsNullOrEmpty(ipList))
        {
            string splittedIPAddress = ipList.Split(',')[0];
            var splittedAddress = splittedIPAddress.Split(':');
            if (splittedAddress.Length > 1)
            {
                return int.Parse(splittedAddress[1]);
            }
        }

        // Returrn ip address
        return request.HttpContext?.Connection?.RemotePort;
    }

    #endregion
}
