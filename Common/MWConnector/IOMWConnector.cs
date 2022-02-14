using System;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.HTTP.Enumerations;
using IOBootstrap.NET.Common.HTTP.Utils;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Utilities;

namespace IOBootstrap.Net.Common.MWConnector
{
    public class IOMWConnector : IOMWConnectorProtocol
    {

        #region Properties

        private IOAESUtilities AESUtilities;
        private IOHTTPClient HTTPClient;
        private ILogger<IOLoggerType> Logger;

        #endregion

        public IOMWConnector(ILogger<IOLoggerType> logger, IConfiguration configuration)
        {
            Logger = logger;

            byte[] keyBytes = Convert.FromBase64String(configuration.GetValue<string>(IOMWConfigurationConstants.EncryptionKey));
            byte[] ivBytes = Convert.FromBase64String(configuration.GetValue<string>(IOMWConfigurationConstants.EncryptionIV));
            AESUtilities = new IOAESUtilities(keyBytes, ivBytes);

            string baseURL = configuration.GetValue<string>(IOMWConfigurationConstants.MiddlewareURL);
            string authorization  = configuration.GetValue<string>(IOMWConfigurationConstants.AuthorizationKey);
            HTTPClient = new IOHTTPClient(baseURL, logger);
            HTTPClient.AddHeader(IORequestHeaderConstants.Authorization, authorization);
            HTTPClient.SetContentType("text/plain");
            HTTPClient.SetRequestMethod(IOHTTPClientRequestMethods.POST);
        }

        public TObject Get<TObject>(string path, Object request) where TObject : IOResponseModel, new()
        {
            TObject jsonObject = null;
            string serializedRequest = JsonSerializer.Serialize(request, new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

            string encryptedBody = AESUtilities.Encrypt(serializedRequest);
            HTTPClient.SetPostBody(encryptedBody);
            Task task = HTTPClient.Call(path, (bool status, string response, HttpResponseHeaders headers) =>
            {
                try
                {
                    string decryptedResult = response;
                    if (headers.Contains(IORequestHeaderConstants.IsEncrypted))
                    {
                        decryptedResult = AESUtilities.Decrypt(response);
                    }

                    if (!String.IsNullOrEmpty(decryptedResult))
                    {
                        jsonObject = JsonSerializer.Deserialize<TObject>(decryptedResult);
                    }
                } 
                catch (Exception ex)
                {
                    // Log call
                    Logger.LogError(ex, ex.Message + '\n' + '\n' + ex.StackTrace);
                }
            });

            task.Wait();

            if (jsonObject == null && !jsonObject.Status.Success)
            {
                // Log call
                jsonObject = null;
            }

            return jsonObject;
        }
    }
}
