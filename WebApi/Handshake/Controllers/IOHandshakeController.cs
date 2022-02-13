using System;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.Handshake;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.WebApi.Handshake.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.WebApi.Handshake.Controllers
{
    public class IOHandshakeController<TViewModel> : IOController<TViewModel> where TViewModel : IOHandshakeViewModel, new()
    {        
        #region Controller Lifecycle

        public IOHandshakeController(IConfiguration configuration, 
                                    IWebHostEnvironment environment, 
                                    ILogger<IOLoggerType> logger) : base(configuration, environment, logger)
        {
        }

        #endregion

        #region Handshake Methods

        [IORequireHTTPS]
        [HttpGet]
        public virtual HandshakeResponseModel Index()
        {
            // Get public key
            Tuple<string, string> publicKey = ViewModel.GetPuplicKey();

            // Obtain key id
            string keyID = "";
            IOCacheObject keyIDCacheObject = IOCache.GetCachedObject(IOCacheKeys.RSAPrivateKeyIDCacheKey);
            if (keyIDCacheObject != null) 
            {
                keyID = (string)keyIDCacheObject.Value;
            }

            // Create and return response
            return new HandshakeResponseModel(publicKey.Item2, publicKey.Item1, keyID);
        }

        [IORequireHTTPS]
        [HttpGet]
        public virtual IOResponseModel CheckSession()
        {
            return new IOResponseModel();
        }

        #endregion
    }
}
