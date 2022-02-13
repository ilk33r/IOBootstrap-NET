using System;
using IOBootstrap.NET.MW.DataAccess.Context;

namespace IOBootstrap.NET.MW.Application
{
    public class IOMWStartupDefault : IOMWStartup<IODatabaseContextDefaultImpl>
    {
        #region Initialization Methods

        public IOMWStartupDefault(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
        }

        #endregion
    }
}
