using System;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.Clients;
using IOBootstrap.NET.Common.Models.Clients;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Core.Controllers
{
    public abstract class IOBackOfficeController<TViewModel, TDBContext> : IOController<TViewModel, TDBContext> where TViewModel : IOBackOfficeViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Controller Lifecycle

        public IOBackOfficeController(IConfiguration configuration,
                                      TDBContext databaseContext,
                                      IWebHostEnvironment environment,
                                      ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Update view model request value
            ViewModel.Request = Request;

            if (!ViewModel.IsBackOffice()) 
            {
                throw new IOInvalidPermissionException();
            }

            base.OnActionExecuting(context);
        }

        #endregion

        #region Client Methods

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.Admin)]
        [HttpPost]
        public IOClientAddResponseModel AddClient([FromBody] IOClientAddRequestModel requestModel)
        {
            // Obtain client info from view model
            IOClientInfoModel clientInfo = ViewModel.CreateClient(requestModel.ClientDescription, requestModel.RequestCount);

            // Create and return response
            return new IOClientAddResponseModel(clientInfo);
        }

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.Admin)]
        [HttpPost]
        public IOResponseModel DeleteClient([FromBody] IOClientDeleteRequestModel requestModel)
        {
            // Check delete client is success
            ViewModel.DeleteClient(requestModel.ClientId);
            
            // Then create and return response
            return new IOResponseModel();
        }

        [IOUserRole(UserRoles.Admin)]
        [HttpGet]
        public IOClientListResponseModel ListClients()
        {
            // Obtain client infos
            List<IOClientInfoModel> clientInfos = ViewModel.GetClients();

            // Create and return response
            return new IOClientListResponseModel(clientInfos);
        }

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.Admin)]
        [HttpPost]
        public IOResponseModel UpdateClient([FromBody] IOClientUpdateRequestModel requestModel)
        {
            // Check update client is success
            ViewModel.UpdateClient(requestModel.ClientId, requestModel.ClientDescription, requestModel.IsEnabled, requestModel.RequestCount, requestModel.MaxRequestCount);
            
            // Then create and return response
            return new IOResponseModel();
        }

        #endregion
    }
}
