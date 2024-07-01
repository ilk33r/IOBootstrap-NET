using System;

namespace IOBootstrap.NET.Batch.Base.Common.Models;

public class BatchConfigurationModel
{

    public required string IOAPNSApiURL { get; set; }
    public required string IOAPNSAuthKeyID { get; set; }
    public required string IOAPNSBundleID { get; set; }
    public required string IOAPNSKeyFilePath { get; set; }
    public required string IOAPNSTeamID { get; set; }
    public required string IOFirebaseApiUrl { get; set; }
    public required string IOFirebaseToken { get; set; }
    public required string IOFunctionsPushNotificationControllerName { get; set; }
    public required string IOMWAuthorizationKey { get; set; }
    public required string IOMWEncryptionKey { get; set; }
    public required string IOMWEncryptionIV { get; set; }
    public required string IOMWURL { get; set; }
}
