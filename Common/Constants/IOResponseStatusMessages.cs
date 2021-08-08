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
            {401, "Invalid permission."},
            {404, "Endpoint could not found."},
            {405, "Https required."},
            {406, "Invalid client."},
            {600, "Authorization failed."},
            {630, "Key expired."},
            {700, "User exists."},
            {900, "General exception."}
        };

        #endregion

        #region Status Messages

        public static int OK = 200;
        public static int BAD_REQUEST = 400;
        public static int INVALID_PERMISSION = 401;
        public static int ENDPOINT_FAILURE = 404;
        public static int HTTPS_REQUIRED = 405;
        public static int INVALID_CLIENT = 406;
        public static int AUTHORIZATION_FAILED = 600;
        public static int INVALID_KEY_ID = 630;
        public static int USER_EXISTS = 700;
        public static int GENERAL_EXCEPTION = 900;

        #endregion
    }
}
