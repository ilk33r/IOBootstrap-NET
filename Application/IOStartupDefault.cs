using System;

namespace IOBootstrap.NET.Application
{
    public class IOStartupDefault : IOStartup
    {
        #region Initialization Methods

        public IOStartupDefault(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
        }

        #endregion
    }
}
