using System;
using IOBootstrap.NET.Common.Models.Firebase;
using IOBootstrap.NET.Core.HTTP.Enumerations;
using IOBootstrap.NET.Core.HTTP.Utils;
using IOBootstrap.NET.Core.Logger;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Core.Firebase
{

    public delegate void FirebaseSendNotificationHandler(bool status, FirebaseResponseModel responseObject);

    public class FirebaseUtils
    {

        #region Properties

        private string FirebaseApiUrl;
        private string FirebaseToken;
        private ILogger<IOLoggerType> Logger;

        #endregion

        #region Initialization Methods

        public FirebaseUtils(string firebaseApiUrl, string firebaseToken, ILogger<IOLoggerType> logger)
        {
            // Setup properties
            FirebaseApiUrl = firebaseApiUrl;
            FirebaseToken = firebaseToken;
            Logger = logger;
        }

        #endregion

        #region Utility Methods

        public void SendNotifications(FirebaseModel firebaseData, FirebaseSendNotificationHandler callback)
        {
            // Create http client
            IOHTTPClient httpClient = new IOHTTPClient(FirebaseApiUrl);

            // Add headers
            string authorization = "key=" + FirebaseToken;
            httpClient.AddHeader("Authorization", authorization);
            httpClient.AddAcceptHeader("application/json");

            // Set request method
            httpClient.SetRequestMethod(IOHTTPClientRequestMethods.POST);

            // Set request body
            httpClient.SetPostBody(firebaseData);

            // Call http client
            httpClient.CallJSON<FirebaseResponseModel>((bool status, FirebaseResponseModel responseObject) =>
            {
                if (status) 
                {
                    Logger.LogInformation("Firebase api called successfully.\nSuccess: {0}\nFailure: {1}", new object[] { responseObject.success.ToString(), responseObject.failure.ToString() });
                    callback(status, responseObject);
                } else {
                    Logger.LogError("Firebase api call failed.");
                    callback(status, null);
                }
            });
        }

        #endregion

    }
}
