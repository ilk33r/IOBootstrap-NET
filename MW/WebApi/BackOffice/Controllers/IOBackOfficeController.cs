using System;
using IOBootstrap.Net.Common.Messages.MW;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.Clients;
using IOBootstrap.NET.Common.Models.Clients;
using IOBootstrap.NET.MW.Core.Controllers;
using IOBootstrap.NET.MW.DataAccess.Context;
using IOBootstrap.NET.MW.WebApi.BackOffice.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.MW.WebApi.BackOffice.Controllers
{
    public abstract class IOBackOfficeController<TViewModel, TDBContext> : IOMWController<TViewModel, TDBContext> where TViewModel : IOBackOfficeViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {
        public IOBackOfficeController(IConfiguration configuration, 
                                      TDBContext databaseContext, 
                                      IWebHostEnvironment environment, 
                                      ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOMWObjectResponseModel<IOClientInfoModel> AddClient([FromBody] IOClientAddRequestModel requestModel)
        {
            // Obtain client info from view model
            IOClientInfoModel clientInfo = ViewModel.CreateClient(requestModel.ClientDescription, requestModel.RequestCount);
            return new IOMWObjectResponseModel<IOClientInfoModel>(clientInfo);
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOResponseModel DeleteClient([FromBody] IOClientDeleteRequestModel requestModel)
        {
            // Check update client is success
            if (ViewModel.DeleteClient(requestModel))
            {
                return new IOResponseModel();
            }
            
            // Then create and return response
            return new IOResponseModel(IOResponseStatusMessages.UnkownException);
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOMWListResponseModel<IOClientInfoModel> ListClients([FromBody] IOMWFindRequestModel requestModel)
        {
            IList<IOClientInfoModel> clients = ViewModel.GetClients();
            return new IOMWListResponseModel<IOClientInfoModel>(clients);
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOResponseModel UpdateClient([FromBody] IOClientUpdateRequestModel requestModel)
        {
            // Check update client is success
            if (ViewModel.UpdateClient(requestModel))
            {
                return new IOResponseModel();
            }
            
            // Then create and return response
            return new IOResponseModel(IOResponseStatusMessages.UnkownException);
        }
    }
}
