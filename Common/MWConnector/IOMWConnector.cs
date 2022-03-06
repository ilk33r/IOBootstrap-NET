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
using static IOBootstrap.Net.Common.MWConnector.IOMWConnectorProtocol;

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
            string decryptedResult = null;
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
                    decryptedResult = response;
                    if (headers.Contains(IORequestHeaderConstants.IsEncrypted))
                    {
                        decryptedResult = AESUtilities.Decrypt(response);
                    }
                } 
                catch (Exception ex)
                {
                    // Log call
                    Logger.LogError(ex, ex.Message + '\n' + '\n' + ex.StackTrace);
                }
            });

            task.Wait();
            
            
            if (!String.IsNullOrEmpty(decryptedResult))
            {
                try
                {
                    jsonObject = JsonSerializer.Deserialize<TObject>(decryptedResult, new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true,
                        DefaultIgnoreCondition = JsonIgnoreCondition.Never
                    });
                }
                catch (Exception ex)
                {
                    // Log call
                    Logger.LogError(ex, ex.Message + '\n' + '\n' + ex.StackTrace);
                }
            }

            return jsonObject;
        }

        public bool HandleResponse<TObject>(TObject response, IOMWConnectorResponseHandler handler) where TObject : IOResponseModel, new()
        {
            if (response == null)
            {
                handler(0);
                return false;
            }

            if (!response.Status.Success)
            {
                handler(response.Status.Code);
                return false;
            }

            return true;
        }
    }
}
