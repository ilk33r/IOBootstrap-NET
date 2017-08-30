using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Entities.Clients;
using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.WebApi.BackOffice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace IOBootstrap.NET.WebApi.BackOffice.Controller
{
    public abstract class IOBackOfficeController<TLogger>: IOController<TLogger>
    {

        #region Controller Lifecycle

        public IOBackOfficeController(ILoggerFactory factory, ILogger<TLogger> logger, IConfiguration configuration, IIODatabase database)
            : base(factory, logger, configuration, database)
        {
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            // Check is not back office
            if (!this.IsBackOffice())
            {
                // Obtain response model
                IOResponseModel responseModel = this.Error400("This ip restricted.");

                // Override response
                context.Result = new JsonResult(responseModel);

                // Do nothing
                return;
            }

            base.OnActionExecuting(context);
        }

        #endregion

        #region Client Methods

        [HttpPost]
        public IOClientAddResponseModel AddClient([FromBody] IOClientAddRequestModel requestModel)
        {
            // Create a client entity
            IOClientsEntity clientEntity = IOClientsEntity.CreateClient(_database, requestModel.ClientDescription);

            // Write client to database
            _database.InsertEntity(clientEntity)
                     .Subscribe();

            // Create client info
            var clientInfos = new IOClientBackOfficeInfoModel(clientEntity.ID, clientEntity.ClientId, clientEntity.ClientSecret);

            // Create and return response
            return new IOClientAddResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), clientInfos);
		}

        [HttpPost]
        public IOResponseModel DeleteClient([FromBody] IOClientDeleteRequestModel requestModel) 
        {
            // Obtain realm instance 
            Realm realm = _database.GetRealmForMainThread();

            // Obtain client entity
            IOClientsEntity clientEntity = realm.Find<IOClientsEntity>(requestModel.ClientId);

            // Check client entity is not null
            if (clientEntity != null) {
				// Begin write transaction
				Transaction realmTransaction = realm.BeginWrite();

				// Delete all entity
				realm.Remove(clientEntity);

				// Write transaction
				realmTransaction.Commit();

                // Then return response
                return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK));
            }

            // Dispose realm
            realm.Dispose();

            // Return bad request
            this.Response.StatusCode = 400;
            return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.BAD_REQUEST, "Client not found."));
        }

        public IOClientListResponseModel ListClients()
        {
            // Obtain client infos
            var clientInfos = IOClientsEntity.GetClients(_database);

            // Create and return response
            return new IOClientListResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), clientInfos);
        }

        #endregion

        #region Helper Methods

        public bool IsBackOffice()
        {
            // Obtain user ip address
            string userIP = IOCommonHelpers.GetUserIP(this.Request);

            // Log call
            _logger.LogDebug("User IP: {0}", userIP);

            // Check user ip is not local address
            if (userIP.Equals(_configuration.GetValue<string>("IODefaultLocalIPV4Address")) || userIP.Equals(_configuration.GetValue<string>("IODefaultLocalIPV6Address")))
            {
                // Then return back office
                return true;
            }

            // Return is not back office
            return false;
        }

        #endregion

    }
}
