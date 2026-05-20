using System;
using System.Text.Json;
using Google.Apis.Auth.OAuth2;
using IOBootstrap.NET.Common.HTTP;
using IOBootstrap.NET.Common.HTTP.Enumerations;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Models.Firebase;

namespace IOBootstrap.NET.Common.Firebase;

public class FirebaseUtils
{
    public enum FirebaseUtilsMessageTypes
    {
        Success = 0,
        DeviceNotFound = 1,
        Failure = 2
    }

    #region Properties

    private string APIURL;
    private string ProjectID;
    private ILogger<IOLoggerType>? Logger;
    private GoogleCredential Credential;

    #endregion

    #region Initialization Methods

    public FirebaseUtils(
        ILogger<IOLoggerType>? logger,
        string apiURL,
        string projectID,
        string keyFile
    )
    {
        // Setup properties
        this.Logger = logger;
        this.APIURL = apiURL;
        this.ProjectID = projectID;

        this.Credential = CredentialFactory.FromJson(keyFile, "service_account")
                                .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");
    }

    #endregion

    #region Utility Methods

    public async Task<string> GetAccessTokenAsync()
    {
        return await Credential.UnderlyingCredential
        .GetAccessTokenForRequestAsync(APIURL);
    }

    public async Task<FirebaseUtilsMessageTypes> SendNotificationsAsync(FirebaseModel firebaseData, string token)
    {
        // Create a message
        FirebaseMessageModel message = new FirebaseMessageModel()
        {
            Message = firebaseData
        };

        var url = $"{APIURL}/v1/projects/{ProjectID}/messages:send";

        // Call http client
        IOHTTPClient httpClient = new IOHTTPClient(url, Logger!);

        // Set request method
        httpClient.SetRequestMethod(IOHTTPClientRequestMethods.POST);
        httpClient.AddHeader("Authorization", String.Format("Bearer {0}", token));

        // Set request body
        httpClient.SetBody(message);

        // Call http client
        var response = await httpClient.CallJSONAsync<FirebaseResponseModel>();

        // Check result
        if (response.Item1 && response.Item2?.Error == null)
        {
            Logger?.LogInformation("Firebase api called successfully.");
            return FirebaseUtilsMessageTypes.Success;
        }

        if (response.Item2?.Error != null && response.Item2.Error.Status == "INVALID_ARGUMENT")
        {
            var unregisteredDevice = response.Item2.Error.Details?.Where(d => d.ErrorCode == "UNREGISTERED") ?? [];
            if (unregisteredDevice.Count() > 0)
            {
                Logger?.LogError("Firebase api call failed. Device not found.");
                return FirebaseUtilsMessageTypes.DeviceNotFound;
            }

            return FirebaseUtilsMessageTypes.DeviceNotFound;
        }

        Logger?.LogError("Firebase api call failed.");
        return FirebaseUtilsMessageTypes.Failure;
    }

    #endregion
}
