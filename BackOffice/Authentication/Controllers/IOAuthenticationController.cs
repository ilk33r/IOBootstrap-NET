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

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 10)]
    [IOValidateRequestModel]
    [IOEncryptionRequired]
    [IONonceRequired]
    [IOIgnorePasswordExpire]
    [HttpPost("[action]")]
    public virtual async Task<IOAuthenticationResponseModel> Authenticate([FromBody] IOAuthenticationRequestModel requestModel)
    {
        // Check if authentication result is true
        return await ViewModel.Authenticate(requestModel.UserName ?? "", requestModel.Password ?? "");
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 20)]
    [IOValidateRequestModel]
    [IOEncryptionRequired]
    [IONonceRequired]
    [IOIgnorePasswordExpire]
    [HttpPost("[action]")]
    public virtual IOCheckTokenResponseModel CheckToken([FromBody] IOCheckTokenRequestModel requestModel)
    {
        // Check if authentication result is true
        return ViewModel.CheckToken(requestModel.Token ?? "");
    }

    #endregion
}
