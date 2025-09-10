using System;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.Handshake;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.WebApi.Handshake.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IOBootstrap.NET.WebApi.Handshake.Controllers;

public class IOHandshakeController<TViewModel, TDBContext> : IOController<TViewModel, TDBContext>
where TDBContext : IODatabaseContext<TDBContext>
where TViewModel : IOHandshakeViewModel<TDBContext>, new()
{
    #region Controller Lifecycle

    public IOHandshakeController(IConfiguration configuration,
                                IWebHostEnvironment environment,
                                ILogger<IOLoggerType> logger,
                                TDBContext databaseContext) : base(configuration, environment, logger, databaseContext)
    {
    }

    #endregion

    [ApiExplorerSettings(IgnoreApi = true)]
    public override void CheckIsMaintenanceMode(ActionExecutingContext context)
    {
    }

    #region Handshake Methods

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 15)]
    [HttpGet("[action]")]
    public virtual async Task<HandshakeResponseModel> Index()
    {
        // Get public key
        Tuple<string, string> publicKey = await ViewModel.GetPuplicKey();

        // Obtain key id
        string keyID = "";
        IOCacheObject? keyIDCacheObject = await IOCache.GetCachedObjectAsync(IOCacheKeys.RSAPrivateKeyIDCacheKey);
        if (keyIDCacheObject != null)
        {
            keyID = (string)keyIDCacheObject.Value;
        }

        if (String.IsNullOrEmpty(keyID))
        {
            await IOCache.InvalidateCacheAsync(IOCacheKeys.RSAPrivateKeyCacheKey);
            await IOCache.InvalidateCacheAsync(IOCacheKeys.RSAPrivateKeyIDCacheKey);
            throw new IOEncryptionRequiredException();
        }

        // Create and return response
        return new HandshakeResponseModel(publicKey.Item2, publicKey.Item1, keyID);
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 15)]
    [HttpGet("[action]")]
    public virtual IOResponseModel CheckSession()
    {
        return new IOResponseModel();
    }

    #endregion
}
