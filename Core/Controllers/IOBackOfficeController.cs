using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Entities.Clients;
using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IOBootstrap.NET.Core.Controllers
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

        public IOResponseModel AddClient()
        {
            return this.Error400("AddClients.");
        }

        public IOClientListResponseModel ListClients()
        {
            // Obtain realm 
            Realm realm = _database.GetRealmForMainThread();

            // Obtain clients from realm
            IQueryable<IOClientsEntity> clients = realm.All<IOClientsEntity>();

            // Create list for clients
            List<IOClientBackOfficeInfoModel> clientInfos = new List<IOClientBackOfficeInfoModel>();

            // Loop throught clients
            foreach(IOClientsEntity client in clients) {
                // Create back office info model
                IOClientBackOfficeInfoModel model = new IOClientBackOfficeInfoModel(client.id, client.clientId, client.clientSecret);

                // Add model to client info list
                clientInfos.Add(model);
            }

            // Create and return response
            return new IOClientListResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), clientInfos);
        }

        #endregion

        #region Helper Methods

        public bool IsBackOffice()
        {
            // Obtain user ip address
            string userIP = IOCommonHelpers.GetUserIP(this.Request);

            // Check user ip is not local address
            if (userIP.Equals("127.0.0.1"))
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
