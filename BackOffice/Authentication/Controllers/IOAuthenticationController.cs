using System;
using IOBootstrap.NET.BackOffice.Authentication.Interfaces;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Authentication;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.BackOffice.Authentication.Controllers;

[IOBackoffice]
public abstract class IOAuthenticationController<TViewModel, TDBContext> : IOController<TViewModel, TDBContext>
where TDBContext : IODatabaseContext<TDBContext>
where TViewModel : IIOAuthenticationViewModel<TDBContext>, new()
{
    #region Controller Lifecycle

    protected IOAuthenticationController(IConfiguration configuration,
                                         IWebHostEnvironment environment,
                                         ILogger<IOLoggerType> logger,
                                         TDBContext databaseContext) : base(configuration, environment, logger, databaseContext)
    {
    }

    #endregion

    #region Authentication Api

    [IOValidateRequestModel]
    [IOEncryptionRequired]
    [HttpPost("[action]")]
    public virtual IOAuthenticationResponseModel Authenticate([FromBody] IOAuthenticationRequestModel requestModel)
    {
        // Check if authentication result is true
        return ViewModel.Authenticate(requestModel.UserName ?? "", requestModel.Password ?? "");
    }

    [IOValidateRequestModel]
    [IOEncryptionRequired]
    [HttpPost("[action]")]
    public virtual IOCheckTokenResponseModel CheckToken([FromBody] IOCheckTokenRequestModel requestModel)
    {
        // Check if authentication result is true
        return ViewModel.CheckToken(requestModel.Token ?? "");
    }

    #endregion
}
