using IOBootstrap.NET.Common.Entities.Clients;
using IOBootstrap.NET.Core.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace IOBootstrap.NET.Core.ViewModels
{
    public class IOViewModel<TDBContext> 
        where TDBContext : IODatabaseContext<TDBContext>
    {

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
			if (_request.Headers.ContainsKey("X-IO-AUTHORIZATION"))
			{
				// Obtain request authorization value
				string requestAuthorization = _request.Headers["X-IO-AUTHORIZATION"];

				// Check authorization code is equal to configuration value
				if (requestAuthorization.Equals(_configuration.GetValue<string>("IOAuthorizationKey")))
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
                    int requestCount = client.RequestCount + 1;
                    int maxRequestCount = client.MaxRequestCount;

                    // Check request counts
                    if (requestCount <= maxRequestCount)
                    {
                        // Update request count
                        client.RequestCount = requestCount;

                        // Update client 
                        _databaseContext.Update(client);
                        _databaseContext.SaveChangesAsync();

                        // Then return client valid
                        return true;   
                    }
				}
			}

			// Then return invalid clients
			return false;
		}

        #endregion

    }
}
