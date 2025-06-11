using System;

namespace IOBootstrap.NET.Common.Constants;

public static class IOExceptionMessages
{

    public const int InvalidRequestCode = 400;
    public const int InvalidPermissionCode = 401;
    public const int InvalidAPICode = 402;
    public const int InvalidCredentialsCode = 403;
    public const int HttpsRequiredCode = 405;
    public const int InvalidPasswordsCode = 407;
    public const int UserDeactivatedCode = 408;
    public const int PasswordExpiredCode = 409;
    public const int CaptchaRequiredCode = 410;
    public const int MaintenanceCode = 503;
    public const int MWConnectionCode = 505;
    public const int UnauthorizedCode = 600;
    public const int InvalidKeyIDCode = 630;
    public const int EncryptionRequiredCode = 631;
    public const int InvalidNonceCode = 632;
    public const int RateLimitExceededCode = 633;
    public const int UserExistsCode = 700;
    public const int UserNotFoundCode = 701;
    public const int ImageNotFoundCode = 702;
    public const int ImageDeleteCode = 703;
    public const int ImageCorruptCode = 704;
    public const int ImageSaveCode = 705;
    public const int ImageFileSizeCode = 706;

    public const string InvalidRequestMessage = "Invalid request.";
    public const string InvalidPermissionMessage = "Invalid permission.";
    public const string InvalidAPIMessage = "Invalid API endpoint.";
    public const string InvalidCredentialsMessage = "Invalid credientals.";
    public const string HttpsRequiredMessage = "Https required.";
    public const string InvalidPasswordsMessage = "Invalid password.";
    public const string UserDeactivatedMessage = "Account deactivated.";
    public const string PasswordExpiredMessage = "Password expired.";
    public const string CaptchaRequiredMessage = "Captcha";
    public const string MaintenanceMessage = "The application is in maintenance.";
    public const string MWConnectionMessage = "MW connection error.";
    public const string UnauthorizedMessage = "Authorization failed.";
    public const string InvalidKeyIDMessage = "Invalid key ID or key expired.";
    public const string EncryptionRequiredMessage = "Encryption required.";
    public const string InvalidNonceMessage = "Invalid nonce.";
    public const string RateLimitExceededMessage = "Rate limit exceeded.";
    public const string UserExistsMessage = "User exists.";
    public const string UserNotFoundMessage = "User not found.";
    public const string ImageNotFoundMessage = "Image not found.";
    public const string ImageDeleteMessage = "Could not delete image in blob container.";
    public const string ImageCorruptMessage = "Could not read image file.";
    public const string ImageSaveMessage = "Could not save image file.";
    public const string ImageFileSizeMessage = "File is too big.";
}
