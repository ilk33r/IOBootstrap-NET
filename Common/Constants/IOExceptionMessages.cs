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
        public const int MaintenanceCode = 503;
        public const int MWConnectionCode = 505;
        public const int UnauthorizedCode = 600;
        public const int InvalidKeyIDCode = 630;
        public const int UserExistsCode = 700;
        public const int UserNotFoundCode = 701;
        public const int ImageNotFoundCode = 702;
        public const int ImageDeleteCode = 703;

        public const string InvalidRequestMessage = "Invalid request.";
        public const string InvalidPermissionMessage = "Invalid permission.";
        public const string InvalidCredentialsMessage = "Invalid credientals.";
        public const string HttpsRequiredMessage = "Https required.";
        public const string InvalidClientMessage = "Invalid client ID.";
        public const string MaintenanceMessage = "The application is in maintenance.";
        public const string MWConnectionMessage = "MW connection error.";
        public const string UnauthorizedMessage = "Authorization failed.";
        public const string InvalidKeyIDMessage = "Invalid key ID or key expired.";
        public const string UserExistsMessage = "User exists.";
        public const string UserNotFoundMessage = "User not found.";
        public const string ImageNotFoundMessage = "Image not found.";
        public const string ImageDeleteMessage = "Could not delete image in blob container.";
    }
}
