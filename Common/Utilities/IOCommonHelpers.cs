using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace IOBootstrap.NET.Common.Utilities
{
    public static class IOCommonHelpers
    {
        #region Application Version

        public static string CurrentVersion
        {
            get
            {
                return Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            }
        }

		#endregion

		#region HTTP Helpers

		public static string GetUserIP(HttpRequest request)
		{
			// Obtain ip list from forwaded
			string ipList = request.Headers["HTTP_X_FORWARDED_FOR"];

			// Check ip list is not null
			if (!string.IsNullOrEmpty(ipList))
			{
				return ipList.Split(',')[0];
			}

            // Returrn ip address
            return request.HttpContext.Connection.RemoteIpAddress.ToString();
		}

		#endregion

	}
}
