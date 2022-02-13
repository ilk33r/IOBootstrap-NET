using System;

namespace IOBootstrap.NET.Common.Routes
{
    public class IORoute
    {

        #region Properties

        public string Action { get; }
        public string Controller { get; }

        #endregion

        #region Initialization Methods

        public IORoute(string action, string controller)
        {
            Action = action;
            Controller = controller;
        }

        #endregion

        #region Helper Methods

        public string GetRouteString() 
        {
            return String.Format("{{controller={0}}}/{{action={1}}}", Controller, Action);
        }

        #endregion
    }
}
