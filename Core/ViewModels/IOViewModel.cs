﻿using System;
using System.Linq;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Core.ViewModels
{
    public abstract class IOViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Publics

        public string ClientId;
        public string ClientDescription;

        #endregion

        #region Properties

        public IConfiguration Configuration { get; set; }
        public TDBContext DatabaseContext { get; set; }
        public IWebHostEnvironment Environment { get; set; }
        public ILogger<IOLoggerType> Logger { get; set; }
        public HttpRequest Request { get; set; }

        #endregion

        #region Initialization Methods

        public IOViewModel()
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
				if (requestAuthorization.Equals(Configuration.GetValue<string>(IOConfigurationConstants.AuthorizationKey)))
				{
					// Then authorization success
					return;
				}
			}

			throw new IOUnAuthorizeException();
		}

        public virtual void CheckClient()
		{
            // Obtain client info
            bool checkClientInfo = Configuration.GetValue<bool>(IOConfigurationConstants.CheckClientInfo);
            if (!checkClientInfo)
            {
                return;
            }

            // Obtain client ID and Secret
            string clientId = (Request.Headers.ContainsKey(IORequestHeaderConstants.ClientId)) ? (string)Request.Headers[IORequestHeaderConstants.ClientId] : "";
            string clientSecret = (Request.Headers.ContainsKey(IORequestHeaderConstants.ClientSecret)) ? (string)Request.Headers[IORequestHeaderConstants.ClientSecret] : "";

            // Find client
            var clientsEntity = DatabaseContext.Clients.Where((arg1) => arg1.ClientId == clientId);

			// Check finded client counts is greater than zero
			if (clientsEntity.Count() > 0)
			{
				// Obtain client
				IOClientsEntity client = clientsEntity.First();

				// Check client secret
                if (client.IsEnabled == 1 && client.ClientSecret == clientSecret)
				{
                    // Obtain request counts
                    long requestCount = client.RequestCount + 1;
                    long maxRequestCount = client.MaxRequestCount;

                    // Check request counts
                    if (requestCount <= maxRequestCount)
                    {
                        // Update request count
                        client.RequestCount = requestCount;

                        // Update properties
                        ClientId = clientId;
                        ClientDescription = client.ClientDescription;

                        // Update client 
                        DatabaseContext.Update(client);
                        DatabaseContext.SaveChanges();

                        // Then return client valid
                        return;   
                    }
				}
			}

			// Then return invalid clients
			throw new IOInvalidClientException();
		}

        public virtual int GetUserRole()
        {
            return (int)UserRoles.SuperAdmin;
        }

        #endregion

    }
}
