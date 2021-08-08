using System;

namespace IOBootstrap.NET.Common.Constants
{
    public static class IOExceptionMessages
    {

        public const int HttpsRequiredCode = 405;
        public const int InvalidCredentialsCode = 403;
        public const int InvalidPermissionCode = 401;
        public const int UnauthorizedCode = 600;
        public const int UserNotFoundCode = 701;

        public const string HttpsRequiredMessage = "Https required.";
        public const string InvalidCredentialsMessage = "Invalid credientals.";
        public const string InvalidPermissionMessage = "Invalid permission.";
        public const string UnauthorizedMessage = "Authorization failed.";
        public const string UserNotFoundMessage = "User not found.";
    }
}
