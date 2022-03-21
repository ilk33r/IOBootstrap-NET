using System;
using IOBootstrap.NET.Common.Messages.MW;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.Messages;
using IOBootstrap.NET.Common.Models.Messages;
using IOBootstrap.NET.MW.Core.Controllers;
using IOBootstrap.NET.MW.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.MW.WebApi.BackOffice.Controllers
{
    public class IOBackOfficeMessagesController<TViewModel, TDBContext> : IOMWController<TViewModel, TDBContext> where TViewModel : IOBackOfficeMessagesViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {
        public IOBackOfficeMessagesController(IConfiguration configuration, 
                                              TDBContext databaseContext, 
                                              IWebHostEnvironment environment, 
                                              ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOMWListResponseModel<IOMessageModel> ListMessages([FromBody] IOMWFindRequestModel requestModel)
        {
            IList<IOMessageModel> messages = ViewModel.GetMessages();
            return new IOMWListResponseModel<IOMessageModel>(messages);
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOMWListResponseModel<IOMessageModel> ListAllMessages([FromBody] IOMWFindRequestModel requestModel)
        {
            IList<IOMessageModel> messages = ViewModel.GetAllMessages();
            return new IOMWListResponseModel<IOMessageModel>(messages);
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOResponseModel AddMessagesItem([FromBody] IOMessageAddRequestModel requestModel)
        {
            ViewModel.AddMessage(requestModel);
            return new IOResponseModel();
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOResponseModel DeleteMessagesItem([FromBody] IOMWFindRequestModel requestModel)
        {
            ViewModel.DeleteMessage(requestModel.ID);
            return new IOResponseModel();
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOResponseModel UpdateMessagesItem([FromBody] IOMessageUpdateRequestModel requestModel)
        {
            ViewModel.UpdateMessage(requestModel);
            return new IOResponseModel();
        }
    }
}
