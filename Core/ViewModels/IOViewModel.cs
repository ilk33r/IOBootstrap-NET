﻿using System;
using System.Linq;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Entities.Clients;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Core.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Core.ViewModels
{
    public class IOViewModel<TDBContext> 
        where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Publics

        public string clientId;
        public string clientDescription;

        #endregion

        #region Properties

        public IConfiguration _configuration { get; set; }
        public TDBContext _databaseContext { get; set; }
        public IHostingEnvironment _environment { get; set; }
        public ILogger _logger { get; set; }
        public HttpRequest _request { get; set; }

        #endregion

        #region Initialization Methods

        public IOViewModel()
        {
        }

		#endregion

		#region Helper Methods

        public virtual bool CheckAuthorizationHeader()
		{
			// Check authorization header key exists
			if (_request.Headers.ContainsKey(IORequestHeaderConstants.Authorization))
			{
				// Obtain request authorization value
				string requestAuthorization = _request.Headers[IORequestHeaderConstants.Authorization];

				// Check authorization code is equal to configuration value
				if (requestAuthorization.Equals(_configuration.GetValue<string>(IOConfigurationConstants.AuthorizationKey)))
				{
					// Then authorization success
					return true;
				}

			}

			return false;
		}

        public virtual bool CheckClient(string clientId, string clientSecret)
		{
            // Find client
            var clientsEntity = _databaseContext.Clients.Where((arg1) => arg1.ClientId == clientId);

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
                        this.clientId = clientId;
                        this.clientDescription = client.ClientDescription;

                        // Update client 
                        _databaseContext.Update(client);
                        _databaseContext.SaveChanges();

                        // Then return client valid
                        return true;   
                    }
				}
			}

			// Then return invalid clients
			return false;
		}

        public virtual int GetUserRole()
        {
            return (int)UserRoles.SuperAdmin;
        }

        #endregion

    }
}
