using System;

namespace IOBootstrap.NET.Core.System
{
    public class IORoute
    {

        #region Properties

        public string action { get; }
        public string controller { get; }

        #endregion

        #region Initialization Methods

        public IORoute(string action, string controller)
        {
            this.action = action;
            this.controller = controller;
        }

        #endregion

    }
}
