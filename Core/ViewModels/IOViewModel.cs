using IOBootstrap.NET.Common.Entities.Clients;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.WebApi.BackOffice.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Realms;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Core.ViewModels
{
    public class IOViewModel
    {

        #region Properties

        public IConfiguration Configuration { get; set; }
        public IIODatabase Database { get; set; }
        public ILogger Logger { get; set; }
        public HttpRequest Request { get; set; }

        #endregion

        #region Initialization Methods

        public IOViewModel()
        {
        }

		#endregion

		#region Helper Methods

        public bool CheckAuthorizationHeader()
		{
			// Check authorization header key exists
			if (this.Request.Headers.ContainsKey("X-IO-AUTHORIZATION"))
			{
				// Obtain request authorization value
				string requestAuthorization = this.Request.Headers["X-IO-AUTHORIZATION"];

				// Check authorization code is equal to configuration value
				if (requestAuthorization.Equals(this.Configuration.GetValue<string>("IOAuthorizationKey")))
				{
					// Then authorization success
					return true;
				}

			}

			return false;
		}

		public bool CheckClient(IOClientInfoModel clientInfo)
		{
			// Obtain realm instance
			Realm realm = this.Database.GetRealmForMainThread();

			// Find client
			var clientsEntity = realm.All<IOClientsEntity>().Where((arg1) => arg1.ClientId == clientInfo.ClientID);

			// Check finded client counts is greater than zero
			if (clientsEntity.Count() > 0)
			{
				// Obtain client
				IOClientsEntity client = clientsEntity.First();

				// Check client secret
				if (client.ClientSecret == clientInfo.ClientSecret)
				{
					// Dispose realm
					realm.Dispose();

					// Then return client valid
					return true;
				}
			}

			// Dispose realm
			realm.Dispose();

			// Then return invalid clients
			return false;
		}

        #endregion

    }
}
