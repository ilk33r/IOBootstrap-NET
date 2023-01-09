using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.Interfaces;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.Core.ViewModels
{
    public abstract class IOFunctionsViewModel<TDBContext> : IOViewModel<TDBContext>, IIOFunctionsViewModel<TDBContext>
    where TDBContext : IODatabaseContext<TDBContext>
    {
        public override void CheckAuthorizationHeader()
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

		public string EncryptResult(string json)
		{
			byte[] keyBytes = Convert.FromBase64String(Configuration.GetValue<string>(IOMWConfigurationConstants.EncryptionKey));
			byte[] ivBytes = Convert.FromBase64String(Configuration.GetValue<string>(IOMWConfigurationConstants.EncryptionIV));
			IOAESUtilities aes = new IOAESUtilities(keyBytes, ivBytes);
			return aes.Encrypt(json);
		}
    }
}
