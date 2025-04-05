using System;
using IOBootstrap.NET.BackOffice.Messages.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Messages;
using IOBootstrap.NET.Common.Models.Messages;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.BackOffice.Messages.Controllers;

[IOBackoffice]
public abstract class IOBackOfficeMessagesController<TViewModel, TDBContext> : IOBackOfficeController<TViewModel, TDBContext>
where TDBContext : IODatabaseContext<TDBContext>
where TViewModel : IOBackOfficeMessagesViewModel<TDBContext>, new()
{

    #region Controller Lifecycle

    protected IOBackOfficeMessagesController(IConfiguration configuration,
                                             IWebHostEnvironment environment,
                                             ILogger<IOLoggerType> logger,
                                             TDBContext databaseContext) : base(configuration, environment, logger, databaseContext)
    {
    }

    #endregion

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 15)]
    [IOIgnorePasswordExpire]
    [IOUserRole(UserRoles.BackOfficeUser)]
    [HttpGet("[action]")]
    public virtual IOListMessagesResponseModel ListMessages()
    {
        // Obtain message items
        IList<IOMessageModel> messages = ViewModel.GetMessages();

        // Create and return response
        return new IOListMessagesResponseModel(messages);
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 15)]
    [IOUserRole(UserRoles.SuperAdmin)]
    [HttpGet("[action]")]
    public IOListMessagesResponseModel ListAllMessages()
    {
        // Obtain message items
        IList<IOMessageModel> messages = ViewModel.GetAllMessages();

        // Create and return response
        return new IOListMessagesResponseModel(messages);
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 15)]
    [IOValidateRequestModel]
    [IOUserRole(UserRoles.SuperAdmin)]
    [HttpPost("[action]")]
    public IOMessageAddResponseModel AddMessagesItem([FromBody] IOMessageAddRequestModel requestModel)
    {
        // Add menu
        ViewModel.AddMessage(requestModel);

        // Create and return response
        return new IOMessageAddResponseModel();
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 15)]
    [IOValidateRequestModel]
    [IOUserRole(UserRoles.SuperAdmin)]
    [HttpPost("[action]")]
    public IOMessageDeleteResponseModel DeleteMessagesItem([FromBody] IOMessageDeleteRequestModel requestModel)
    {
        // Add menu
        ViewModel.DeleteMessage(requestModel.MessageId);

        // Create and return response
        return new IOMessageDeleteResponseModel();
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 15)]
    [IOValidateRequestModel]
    [IOUserRole(UserRoles.SuperAdmin)]
    [HttpPost("[action]")]
    public IOMessageUpdateResponseModel UpdateMessagesItem([FromBody] IOMessageUpdateRequestModel requestModel)
    {
        // Add menu
        ViewModel.UpdateMessage(requestModel);

        // Create and return response
        return new IOMessageUpdateResponseModel();
    }
}
