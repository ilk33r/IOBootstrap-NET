using System;

namespace IOBootstrap.NET.Common.Constants
{
    public static class IORequestHeaderConstants
    {
        #region Properties

        public static string Authorization = "X-IO-AUTHORIZATION";
        public static string AuthorizationToken = "X-IO-AUTHORIZATION-TOKEN";
        public static string ClientId = "X-IO-CLIENT-ID";
        public static string ClientSecret = "X-IO-CLIENT-SECRET";
        public static string IsEncrypted = "X-IO-IS-ENCRYPTED";
        public static string KeyID = "X-KEY-ID";
        public static string SymmetricIV = "X-SYMMETRIC-IV";
        public static string SymmetricKey = "X-SYMMETRIC-KEY";

        #endregion
    }
}
