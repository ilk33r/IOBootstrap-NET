using System;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.Clients;
using IOBootstrap.NET.Common.Models.Clients;
using IOBootstrap.NET.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IOBootstrap.NET.Core.Controllers
{
    [IOBackoffice]
    public abstract class IOBackOfficeController<TViewModel> : IOController<TViewModel> where TViewModel : IOBackOfficeViewModel, new()
    {

        #region Controller Lifecycle

        public IOBackOfficeController(IConfiguration configuration,
                                      IWebHostEnvironment environment,
                                      ILogger<IOLoggerType> logger) : base(configuration, environment, logger)
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
        [HttpPost("[action]")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IOClientAddResponseModel AddClient([FromBody] IOClientAddRequestModel requestModel)
        {
            // Obtain client info from view model
            IOClientInfoModel clientInfo = ViewModel.CreateClient(requestModel);

            // Create and return response
            return new IOClientAddResponseModel(clientInfo);
        }

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.Admin)]
        [HttpPost("[action]")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IOResponseModel DeleteClient([FromBody] IOClientDeleteRequestModel requestModel)
        {
            // Check delete client is success
            ViewModel.DeleteClient(requestModel);
            
            // Then create and return response
            return new IOResponseModel();
        }
        
        [IOUserRole(UserRoles.Admin)]
        [HttpGet("[action]")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IOClientListResponseModel ListClients()
        {
            // Obtain client infos
            IList<IOClientInfoModel> clientInfos = ViewModel.GetClients();

            // Create and return response
            return new IOClientListResponseModel(clientInfos);
        }
        
        [IOValidateRequestModel]
        [IOUserRole(UserRoles.Admin)]
        [HttpPost("[action]")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IOResponseModel UpdateClient([FromBody] IOClientUpdateRequestModel requestModel)
        {
            // Check update client is success
            ViewModel.UpdateClient(requestModel);
            
            // Then create and return response
            return new IOResponseModel();
        }
        
        #endregion
    }
}
