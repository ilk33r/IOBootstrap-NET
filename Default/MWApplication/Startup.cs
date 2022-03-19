using System;
using IOBootstrap.NET.MW.Application;
using IOBootstrap.NET.MW.DataAccess.Context;

namespace IOBootstrap.NET.Default.MW.Application
{
    public class Startup : IOMWStartup<IODatabaseContextDefaultImpl>
    {
        #region Initialization Methods

        public Startup(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
        }

        #endregion
    }
}
