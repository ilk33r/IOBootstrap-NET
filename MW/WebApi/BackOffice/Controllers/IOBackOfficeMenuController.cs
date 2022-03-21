using System;
using IOBootstrap.NET.Common.Messages.MW;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.Menu;
using IOBootstrap.NET.Common.Models.Menu;
using IOBootstrap.NET.MW.Core.Controllers;
using IOBootstrap.NET.MW.DataAccess.Context;
using IOBootstrap.NET.MW.WebApi.BackOffice.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.MW.WebApi.BackOffice.Controllers
{
    public abstract class IOBackOfficeMenuController<TViewModel, TDBContext> : IOMWController<TViewModel, TDBContext> where TViewModel : IOBackOfficeMenuViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {
        public IOBackOfficeMenuController(IConfiguration configuration, 
                                          TDBContext databaseContext, 
                                          IWebHostEnvironment environment, 
                                          ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOResponseModel AddMenuItem([FromBody] IOMenuAddRequestModel requestModel)
        {
            ViewModel.AddMenuItem(requestModel);
            return new IOResponseModel();
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOResponseModel DeleteMenuItem([FromBody] IOMWFindRequestModel requestModel)
        {
            ViewModel.DeleteMenuItem(requestModel.ID);
            return new IOResponseModel();
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOMWListResponseModel<IOMenuListModel> GetMenuTree([FromBody] IOMWFindRequestModel requestModel)
        {
            // Check if authentication result is true
            IList<IOMenuListModel> menuTree = ViewModel.GetMenuTree(requestModel.ID);
            return new IOMWListResponseModel<IOMenuListModel>(menuTree);
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOResponseModel UpdateMenuItem([FromBody] IOMenuUpdateRequestModel requestModel)
        {
            ViewModel.UpdateMenuItem(requestModel);
            return new IOResponseModel();
        }
    }
}
