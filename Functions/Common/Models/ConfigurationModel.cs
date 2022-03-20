using System;

namespace IOBootstrap.NET.Functions.Common.Models
{
    public class ConfigurationModel
    {

        public string IOAPNSApiURL { get; set; }
        public string IOAPNSAuthKeyID { get; set; }
        public string IOAPNSBundleID { get; set; }
        public string IOAPNSKeyFilePath { get; set; }
        public string IOAPNSTeamID { get; set; }
        public string IOFirebaseApiUrl { get; set; }
        public string IOFirebaseToken { get; set; }
        public string IOFunctionsPushNotificationControllerName { get; set; }
        public string IOMWAuthorizationKey { get; set; }
        public string IOMWEncryptionKey { get; set; }
        public string IOMWEncryptionIV { get; set; }
        public string IOMWURL { get; set; }
    }
}
