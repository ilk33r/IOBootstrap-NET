using System;
using System.Text.Json;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Utilities;
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
		public IOAESUtilities aesUtilities;

        #endregion

        #region Initialization Methods

        public IOMWViewModel()
        {
        }

		public virtual void Prepare()
		{
			byte[] keyBytes = Convert.FromBase64String(Configuration.GetValue<string>(IOMWConfigurationConstants.EncryptionKey));
			byte[] ivBytes = Convert.FromBase64String(Configuration.GetValue<string>(IOMWConfigurationConstants.EncryptionIV));
			aesUtilities = new IOAESUtilities(keyBytes, ivBytes);
		}

		#endregion

		#region Encryption

		public string EncryptResult(string resultString)
		{
			// Encrypt result
			return aesUtilities.Encrypt(resultString);
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
