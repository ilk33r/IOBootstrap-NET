using System;
using System.Collections.Generic;

namespace IOBootstrap.NET.Common.Constants
{
    public static class IOResponseStatusMessages
    {

        #region Messages

        public static Dictionary<int, string> Messages = new Dictionary<int, string> {
            {200, "OK"},
            {400, "Invalid request."},
            {404, "Endpoint not found."},
            {500, "Unkown exception."}
        };

        #endregion

        #region Status Messages

        public static int OK = 200;
        public static int BAD_REQUEST = 400;
        public const int EndpointFailure = 404;
        public const int UnkownException = 500;

        #endregion
    }
}
