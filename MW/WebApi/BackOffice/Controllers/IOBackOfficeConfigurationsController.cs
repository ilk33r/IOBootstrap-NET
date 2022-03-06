using System;
using IOBootstrap.Net.Common.Messages.MW;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.Configuration;
using IOBootstrap.NET.Common.Models.Configuration;
using IOBootstrap.NET.MW.Core.Controllers;
using IOBootstrap.NET.MW.DataAccess.Context;
using IOBootstrap.NET.MW.WebApi.BackOffice.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.MW.WebApi.BackOffice.Controllers
{
    public class IOBackOfficeConfigurationsController<TViewModel, TDBContext> : IOMWController<TViewModel, TDBContext> where TViewModel : IOBackOfficeConfigurationsViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {
        public IOBackOfficeConfigurationsController(IConfiguration configuration, 
                                                    TDBContext databaseContext, 
                                                    IWebHostEnvironment environment, 
                                                    ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOResponseModel AddConfigItem([FromBody] IOConfigurationAddRequestModel requestModel)
        {
            ViewModel.AddConfigItem(requestModel);
            return new IOResponseModel();
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOMWObjectResponseModel<IOConfigurationModel> DeleteConfigItem([FromBody] IOMWFindRequestModel requestModel)
        {
            // Add menu
            IOConfigurationModel item = ViewModel.DeleteConfigItem(requestModel.ID);

            // Create and return response
            return new IOMWObjectResponseModel<IOConfigurationModel>(item);
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOMWListResponseModel<IOConfigurationModel> ListConfigurationItems([FromBody] IOMWFindRequestModel requestModel)
        {
            IList<IOConfigurationModel> configurations = ViewModel.ListConfigurationItems();
            return new IOMWListResponseModel<IOConfigurationModel>(configurations);
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOResponseModel UpdateConfigItem([FromBody] IOConfigurationUpdateRequestModel requestModel)
        {
            // Add menu
            ViewModel.UpdateConfigItem(requestModel);

            // Create and return response
            return new IOResponseModel();
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOMWObjectResponseModel<IOConfigurationModel> GetConfigItem([FromBody] IOMWFindRequestModel requestModel)
        {
            IOConfigurationModel configuration = ViewModel.GetConfigItem(requestModel.Where);
            if (configuration == null)
            {
                return new IOMWObjectResponseModel<IOConfigurationModel>(IOResponseStatusMessages.UnkownException);
            }
            
            return new IOMWObjectResponseModel<IOConfigurationModel>(configuration);
        }
    }
}
