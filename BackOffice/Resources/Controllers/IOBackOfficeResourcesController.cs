using System;
using System.Collections.Generic;
using IOBootstrap.NET.BackOffice.Resources.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Messages.Resources;
using IOBootstrap.NET.Common.Models.Resources;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.BackOffice.Resources.Controllers
{
    [IOBackoffice]
    public class IOBackOfficeResourcesController<TViewModel, TDBContext> : IOBackOfficeController<TViewModel, TDBContext> where TViewModel : IOBackOfficeResourcesViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {
        #region Controller Lifecycle

        public IOBackOfficeResourcesController(IConfiguration configuration, TDBContext databaseContext, IWebHostEnvironment environment, ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }

        #endregion

        #region Resource Methods

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpPost]
        public IOResourceAddResponseModel AddResource([FromBody] IOResourceAddRequestModel requestModel)
        {
            // Add menu
            ViewModel.AddResourceItem(requestModel);

            // Create and return response
            return new IOResourceAddResponseModel();
        }

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpPost]
        public IOResourceDeleteResponseModel DeleteResource([FromBody] IOResourceDeleteRequestModel requestModel) 
        {
            // Add menu
            ViewModel.DeleteResourceItem(requestModel.ID);

            // Create and return response
            return new IOResourceDeleteResponseModel();
        }

        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpGet]
        public IOGetResourcesResponseModel GetAllResources()
        {
            // Add menu
            IList<IOResourceModel> resources = ViewModel.GetAllResources();

            // Create and return response
            return new IOGetResourcesResponseModel(resources);
        }

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.CustomUser)]
        [HttpPost]
        public IOGetResourcesResponseModel GetResources([FromBody] IOGetResourcesRequestModel requestModel)
        {
            // Add menu
            IList<IOResourceModel> resources = ViewModel.GetResources(requestModel.ResourceKeys);

            // Create and return response
            return new IOGetResourcesResponseModel(resources);
        }

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.SuperAdmin)]
        [HttpPost]
        public IOResourceUpdateResponseModel UpdateResource([FromBody] IOResourceUpdateRequestModel requestModel) 
        {
            // Add menu
            ViewModel.UpdateResourceItem(requestModel);

            // Create and return response
            return new IOResourceUpdateResponseModel();
        }

        #endregion
    }
}
