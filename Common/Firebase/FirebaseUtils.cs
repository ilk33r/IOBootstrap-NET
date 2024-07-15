using System;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
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

    private ILogger Logger;

    #endregion

    #region Initialization Methods

    public FirebaseUtils(string privateKeyFileName, ILogger logger)
    {
        // Setup properties
        Logger = logger;

        string currentDirectory = Directory.GetCurrentDirectory();
        string privateKeyFile = Path.Combine(currentDirectory,privateKeyFileName);

        try
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(privateKeyFile),
            });
        } 
        catch (Exception e)
        {
            Logger.LogWarning(e.Message);
        }
    }

    #endregion

    #region Utility Methods

    public FirebaseUtilsMessageTypes SendNotifications(FirebaseModel firebaseData)
    {
        // Create a message
        Message message = new Message()
        {
            Notification = new Notification
            {
                Title = firebaseData.Data.Title,
                Body = firebaseData.Data.Message,

            },
            Data = new Dictionary<string, string>()
            {
                { "notificationType", firebaseData.Data.NotificationType },
                { "notificationId", firebaseData.Data.NotificationId.ToString() },
                { "customData", firebaseData.Data.CustomData },
                { "badgeCount", firebaseData.Data.BadgeCount.ToString() },
            },
            Token = firebaseData.To
        };

        // Call http client
        string? result = SendMessage(message);

        // Check result
        if (string.IsNullOrEmpty(result))
        {
            Logger.LogError("Firebase api call failed. Device not found.");
            return FirebaseUtilsMessageTypes.DeviceNotFound;
        }
        else
        {
            Logger.LogInformation("Firebase api called successfully.");
            return FirebaseUtilsMessageTypes.Success;
        }
    }

    #endregion

    #region Helper Methods

    private string? SendMessage(Message message)
    {
        FirebaseMessaging messaging = FirebaseMessaging.DefaultInstance;
        Task<string> task = messaging.SendAsync(message);
        try
        {
            task.Wait();
        } 
        catch (Exception e)
        {
            Logger.LogError(e.Message);
            return null;
        }

        return task.Result;
    }

    #endregion
}
