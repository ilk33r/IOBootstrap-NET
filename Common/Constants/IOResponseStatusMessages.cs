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
            {406, "Invalid client."},
            {630, "Key expired."},
            {700, "User exists."},
            {900, "General exception."}
        };

        #endregion

        #region Status Messages

        public static int OK = 200;
        public static int BAD_REQUEST = 400;
        public const int EndpointFailure = 404;
        public static int INVALID_CLIENT = 406;
        public static int INVALID_KEY_ID = 630;
        public static int USER_EXISTS = 700;
        public static int GENERAL_EXCEPTION = 900;

        #endregion
    }
}
