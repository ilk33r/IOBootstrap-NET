using System;

namespace IOBootstrap.NET.Common.Constants
{
    public static class IOExceptionMessages
    {

        public const int InvalidRequestCode = 400;
        public const int InvalidPermissionCode = 401;
        public const int InvalidCredentialsCode = 403;
        public const int HttpsRequiredCode = 405;
        public const int InvalidClientCode = 406;
        public const int UnauthorizedCode = 600;
        public const int InvalidKeyIDCode = 630;
        public const int UserExistsCode = 700;
        public const int UserNotFoundCode = 701;

        public const string InvalidRequestMessage = "Invalid request.";
        public const string InvalidPermissionMessage = "Invalid permission.";
        public const string InvalidCredentialsMessage = "Invalid credientals.";
        public const string HttpsRequiredMessage = "Https required.";
        public const string InvalidClientMessage = "Invalid client ID.";
        public const string UnauthorizedMessage = "Authorization failed.";
        public const string InvalidKeyIDMessage = "Invalid key ID or key expired.";
        public const string UserExistsMessage = "User exists.";
        public const string UserNotFoundMessage = "User not found.";
    }
}