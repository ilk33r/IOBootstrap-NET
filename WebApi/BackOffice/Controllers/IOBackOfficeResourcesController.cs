﻿using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.WebApi.BackOffice.Models;
using IOBootstrap.NET.WebApi.BackOffice.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.WebApi.BackOffice.Controllers
{
    public class IOBackOfficeResourcesController<TLogger, TViewModel, TDBContext> : IOBackOfficeController<TLogger, TViewModel, TDBContext>
        where TViewModel : IOBackOfficeResourcesViewModel<TDBContext>, new()
        where TDBContext : IODatabaseContext<TDBContext>
    {
        #region Controller Lifecycle

        public IOBackOfficeResourcesController(ILoggerFactory factory,
                                      ILogger<TLogger> logger,
                                      IConfiguration configuration,
                                      TDBContext databaseContext,
                                      IHostingEnvironment environment)
            : base(factory, logger, configuration, databaseContext, environment)
        {
        }

        #endregion

        #region Resource Methods
        

        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpPost]
        public IOResourceAddResponseModel AddResource([FromBody] IOResourceAddRequestModel requestModel)
        {
            // Validate request
            if (requestModel == null
                || String.IsNullOrEmpty(requestModel.ResourceKey))
            {
                // Obtain 400 error
                IOResponseModel error400 = this.Error400("Invalid request data.");

                // Then return validation error
                return new IOResourceAddResponseModel(new IOResponseStatusModel(error400.Status.Code, error400.Status.DetailedMessage));
            }

            // Add menu
            _viewModel.AddResourceItem(requestModel);

            // Create and return response
            return new IOResourceAddResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK));
        }

        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpGet]
        public IOGetResourcesResponseModel GetAllResources()
        {
            // Add menu
            IList<IOResourceModel> resources = _viewModel.GetAllResources();

            // Create and return response
            return new IOGetResourcesResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), resources);
        }

        [IOUserRole(UserRoles.CustomUser)]
        [HttpPost]
        public IOGetResourcesResponseModel GetResources([FromBody] IOGetResourcesRequestModel requestModel)
        {
            // Validate request
            if (requestModel == null
                || requestModel.ResourceKeys == null)
            {
                // Obtain 400 error
                IOResponseModel error400 = this.Error400("Invalid request data.");

                // Then return validation error
                return new IOGetResourcesResponseModel(new IOResponseStatusModel(error400.Status.Code, error400.Status.DetailedMessage));
            }

            // Add menu
            IList<IOResourceModel> resources = _viewModel.GetResources(requestModel.ResourceKeys);

            // Create and return response
            return new IOGetResourcesResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), resources);
        }

        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpPost]
        public IOResourceUpdateResponseModel UpdateResource([FromBody] IOResourceUpdateRequestModel requestModel) {
            // Validate request
            if (requestModel == null
                || String.IsNullOrEmpty(requestModel.ResourceKey))
            {
                // Obtain 400 error
                IOResponseModel error400 = this.Error400("Invalid request data.");

                // Then return validation error
                return new IOResourceUpdateResponseModel(new IOResponseStatusModel(error400.Status.Code, error400.Status.DetailedMessage));
            }

            // Add menu
            _viewModel.UpdateResourceItem(requestModel);

            // Create and return response
            return new IOResourceUpdateResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK));
        }

        #endregion
    }
}