using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Messages.Handshake;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.WebApi.Handshake.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.WebApi.Handshake.Controllers
{
    public class IOHandshakeController<TViewModel, TDBContext> : IOController<TViewModel, TDBContext> where TViewModel : IOHandshakeViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {        
        #region Controller Lifecycle

        public IOHandshakeController(IConfiguration configuration, TDBContext databaseContext, IWebHostEnvironment environment, ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }

        #endregion

        #region Handshake Methods

        [HttpGet]
        public virtual HandshakeResponseModel Index()
        {
            // Get public key
            Tuple<string, string> publicKey = ViewModel.GetPuplicKey();

            // Create and return response
            return new HandshakeResponseModel(IOResponseStatusMessages.OK, publicKey.Item2, publicKey.Item1);
        }

        #endregion
    }
}
