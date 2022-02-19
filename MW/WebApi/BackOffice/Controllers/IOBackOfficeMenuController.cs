using System;
using IOBootstrap.Net.Common.Messages.MW;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Models.Menu;
using IOBootstrap.NET.MW.Core.Controllers;
using IOBootstrap.NET.MW.DataAccess.Context;
using IOBootstrap.NET.MW.WebApi.BackOffice.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.MW.WebApi.BackOffice.Controllers
{
    public class IOBackOfficeMenuController<TViewModel, TDBContext> : IOMWController<TViewModel, TDBContext> where TViewModel : IOBackOfficeMenuViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {
        public IOBackOfficeMenuController(IConfiguration configuration, 
                                          TDBContext databaseContext, 
                                          IWebHostEnvironment environment, 
                                          ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOMWListResponseModel<IOMenuListModel> GetMenuTree([FromBody] IOMWFindRequestModel requestModel)
        {
            // Check if authentication result is true
            IList<IOMenuListModel> menuTree = ViewModel.GetMenuTree(requestModel.ID);
            return new IOMWListResponseModel<IOMenuListModel>(menuTree);
        }
    }
}
