﻿using System;
using IOBootstrap.NET.Common.HTTP.Enumerations;
using IOBootstrap.NET.Common.HTTP.Utils;
using IOBootstrap.NET.Common.Models.Firebase;

namespace IOBootstrap.NET.Common.Firebase
{

    public class FirebaseUtils
    {
        public enum FirebaseUtilsMessageTypes
        {
            Success = 0,
            DeviceNotFound = 1,
            Failure = 2
        }

        #region Properties

        private string FirebaseApiUrl;
        private string FirebaseToken;
        private ILogger Logger;

        #endregion

        #region Initialization Methods

        public FirebaseUtils(string firebaseApiUrl, string firebaseToken, ILogger logger)
        {
            // Setup properties
            FirebaseApiUrl = firebaseApiUrl;
            FirebaseToken = firebaseToken;
            Logger = logger;
        }

        #endregion

        #region Utility Methods

        public FirebaseUtilsMessageTypes SendNotifications(FirebaseModel firebaseData)
        {
            // Create http client
            IOHTTPClient httpClient = new IOHTTPClient(FirebaseApiUrl, Logger);
            httpClient.IgnoreNullValues = true;

            // Add headers
            string authorization = "key=" + FirebaseToken;
            httpClient.AddAuthorizationHeader(authorization);
            httpClient.AddAcceptHeader("application/json");

            // Set request method
            httpClient.SetRequestMethod(IOHTTPClientRequestMethods.POST);

            // Set request body
            httpClient.SetPostBody(firebaseData);

            // Call http client
            FirebaseResponseModel response = httpClient.CallJSONSync<FirebaseResponseModel>();
            if (response != null && response.Success == 1) 
            {
                Logger.LogInformation("Firebase api called successfully.");
                return FirebaseUtilsMessageTypes.Success;
            }

            if (response != null && response.Failure == 1 && response.Results.Count > 0 && response.Results[0].Error == "InvalidRegistration")
            {
                Logger.LogError("Firebase api call failed. Device not found.");
                return FirebaseUtilsMessageTypes.DeviceNotFound;
            }

            Logger.LogError("Firebase api call failed.");
            return FirebaseUtilsMessageTypes.Failure;
        }

        #endregion

    }
}
