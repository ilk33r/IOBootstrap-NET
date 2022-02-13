using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.MW.DataAccess.Context;

namespace IOBootstrap.NET.MW.Core.ViewModels
{
    public abstract class IOMWViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {
        #region Properties

        public IConfiguration Configuration { get; set; }
        public TDBContext DatabaseContext { get; set; }
        public IWebHostEnvironment Environment { get; set; }
        public ILogger<IOLoggerType> Logger { get; set; }
        public HttpRequest Request { get; set; }

        #endregion

        #region Initialization Methods

        public IOMWViewModel()
        {
        }

		#endregion

		#region Helper Methods

        public virtual void CheckAuthorizationHeader()
		{
			// Check authorization header key exists
			if (Request.Headers.ContainsKey(IORequestHeaderConstants.Authorization))
			{
				// Obtain request authorization value
				string requestAuthorization = Request.Headers[IORequestHeaderConstants.Authorization];

				// Check authorization code is equal to configuration value
				if (requestAuthorization.Equals(Configuration.GetValue<string>(IOMWConfigurationConstants.AuthorizationKey)))
				{
					// Then authorization success
					return;
				}
			}

			throw new IOUnAuthorizeException();
		}

        #endregion
    }
}
