using System;
using System.IO;
using System.Text;
using System.Text.Json;
using IOBootstrap.NET.Common.Models.APNS;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.HTTP.Enumerations;
using IOBootstrap.NET.Core.HTTP.Utils;
using IOBootstrap.NET.Core.Logger;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace IOBootstrap.NET.Core.APNS
{
    public class APNSHttpServiceUtils
    {
        public enum APNSHttpServiceUtilsMessageTypes
        {
            Success = 0,
            DeviceNotFound = 1,
            Failure = 2
        }

        private const string EndpointFormat = "{0}/3/device/{1}";
        private const string JWTHeaderAndBody = "{0}.{1}";

        #region Properties

        private string APNSApiUrl;
        private string APNSAuthKeyID;
        private string APNSBundleID;
        private string APNSKeyFilePath;
        private string APNSTeamID;
        private ILogger<IOLoggerType> Logger;

        #endregion

        #region Initialization Methods

        public APNSHttpServiceUtils(string apnsApiUrl, string apnsAuthKeyID, string apnsBundleID, string apnsKeyFilePath, string apnsTeamID, ILogger<IOLoggerType> logger)
        {
            // Setup properties
            APNSApiUrl = apnsApiUrl;
            APNSAuthKeyID = apnsAuthKeyID;
            APNSBundleID = apnsBundleID;
            APNSKeyFilePath =  apnsKeyFilePath;
            APNSTeamID = apnsTeamID;
            Logger = logger;
        }

        #endregion

        #region Utility Methods

        public APNSHttpServiceUtilsMessageTypes SendNotifications(APNSSendPayloadModel payloadData)
        {
            // Create http client
            string endpoint = String.Format(EndpointFormat, APNSApiUrl, payloadData.DeviceToken);
            IOHTTPClient httpClient = new IOHTTPClient(endpoint, Logger);
            httpClient.UseHttp2 = true;
            httpClient.IgnoreNullValues = true;

            // Add headers
            httpClient.AddHeader("apns-topic", APNSBundleID);
            // httpClient.AddAcceptHeader("application/json");

            // Create JWT authorization
            long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            APNSJWTHeaderModel jwtHeaderModel = new APNSJWTHeaderModel(APNSAuthKeyID);
            APNSJWTBodyModel jwtBodyModel = new APNSJWTBodyModel(APNSTeamID, currentTime);
            string jwt = CreateJWTAuthorization(jwtHeaderModel, jwtBodyModel);
            httpClient.AddAuthorizationHeader("bearer " + jwt);
            
            // Set request method
            httpClient.SetRequestMethod(IOHTTPClientRequestMethods.POST);

            // Set request body
            httpClient.SetPostBody(payloadData.Payload);

            // Call http client
            APNSResponseModel response = httpClient.CallJSONSync<APNSResponseModel>();
            if (response == null) 
            {
                Logger.LogInformation("APNS api called successfully.");
                return APNSHttpServiceUtilsMessageTypes.Success;
            }

            if (response != null && response.Reason.Equals("BadDeviceToken"))
            {
                Logger.LogError("APNS api call failed. Device not found.");
                return APNSHttpServiceUtilsMessageTypes.DeviceNotFound;
            }

            Logger.LogError("APNS api call failed.");
            return APNSHttpServiceUtilsMessageTypes.Failure;
        }

        #endregion

        #region Helper Methods

        private string CreateJWTAuthorization(APNSJWTHeaderModel jwtHeader, APNSJWTBodyModel jwtBody)
        {
            string serializedJWTHeader = JsonSerializer.Serialize(jwtHeader);
            string serializedJWTBody = JsonSerializer.Serialize(jwtBody);
            string base64EncodedJWTHeader = TrimmedBase64String(serializedJWTHeader);
            string base64EncodedJWTBody = TrimmedBase64String(serializedJWTBody);
            string jwtHeaderAndBody = String.Format(JWTHeaderAndBody, base64EncodedJWTHeader, base64EncodedJWTBody);
            string jwtSignature = "";

            ECPrivateKeyParameters privateKey = GetPrivateKey();

            try 
            {
                byte[] message = Encoding.Default.GetBytes(jwtHeaderAndBody);

                ISigner signer = SignerUtilities.GetSigner("SHA-256withECDSA");
                signer.Init(true, privateKey);
                signer.BlockUpdate(message, 0, message.Length);

                // Hash the data
                byte[] sigBytes = signer.GenerateSignature();

                // base64 encode
                jwtSignature = Convert.ToBase64String(sigBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
                Logger.LogInformation("Apple Push Signature Created message bytes {0}", message.Length);
            }
            catch  (Exception ex)
            {
                Logger.LogError(ex.Message);
            }

            return String.Format(JWTHeaderAndBody, jwtHeaderAndBody, jwtSignature);
        }

        private ECPrivateKeyParameters GetPrivateKey()
        {
            ECPrivateKeyParameters privateKey = null;

            try {
                Logger.LogInformation("Apple Private Key Path {0}", APNSKeyFilePath);

                // Read key file
                using (StreamReader reader = File.OpenText(APNSKeyFilePath))
                {
                    // Load client certificate
                    PemReader keyFileReader = new PemReader(reader);
                    privateKey = (ECPrivateKeyParameters)keyFileReader.ReadObject();
                }
            }
            catch  (Exception ex)
            {
                Logger.LogError("ECPrivateKeyParameters exception {0}", ex.Message);
            }

            // Return certificate
            return privateKey;
        }

        private string TrimmedBase64String(string plainString)
        {
            return IOBase64Utilities.Base64Encode(plainString).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

        #endregion
    }
}
