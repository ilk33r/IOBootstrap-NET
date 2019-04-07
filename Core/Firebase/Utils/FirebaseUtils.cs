using System;
using IOBootstrap.NET.Core.Firebase.Models;
using IOBootstrap.NET.Core.HTTP.Enumerations;
using IOBootstrap.NET.Core.HTTP.Utils;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Core.APNS.Utils
{

    public delegate void FirebaseSendNotificationHandler(bool status, FirebaseResponseModel responseObject);

    public class FirebaseUtils
    {

        #region Properties

        private string firebaseApiUrl;
        private string firebaseToken;
        private ILogger logger;

        #endregion

        #region Initialization Methods

        public FirebaseUtils(string firebaseApiUrl, string firebaseToken, ILogger logger)
        {
            // Setup properties
            this.firebaseApiUrl = firebaseApiUrl;
            this.firebaseToken = firebaseToken;
            this.logger = logger;
        }

        #endregion

        #region Utility Methods

        public void SendNotifications(FirebaseModel firebaseData, FirebaseSendNotificationHandler callback)
        {
            // Create http client
            IOHTTPClient httpClient = new IOHTTPClient(this.firebaseApiUrl);

            // Add headers
            string authorization = "key=" + this.firebaseToken;
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
                    this.logger.LogInformation("Firebase api called successfully.\nSuccess: {0}\nFailure: {1}", new object[] { responseObject.success.ToString(), responseObject.failure.ToString() });
                    callback(status, responseObject);
                } else {
                    this.logger.LogError("Firebase api call failed.");
                    callback(status, null);
                }
            });
        }

        #endregion

    }
}
