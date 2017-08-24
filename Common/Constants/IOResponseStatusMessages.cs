using System;

namespace IOBootstrap.NET.Common.Constants
{
    public static class IOResponseStatusMessages
    {

        #region Messages

        public static String[] Messages = {
            "",
            "Endpoint could not found.",
            "Bad request",
            "Invalid clients"
        };

        #endregion

        #region Status Messages

        public static int OK = 0;
        public static int ENDPOINT_FAILURE = 1;
        public static int BAD_REQUEST = 2;
        public static int INVALID_CLIENTS = 3;

        #endregion
    }
}
