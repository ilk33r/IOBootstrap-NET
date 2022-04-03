using System;
using System.Text.Json;
using IOBootstrap.NET.Common.APNS;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Firebase;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.MW;
using IOBootstrap.NET.Common.Models.APNS;
using IOBootstrap.NET.Common.Models.Firebase;
using IOBootstrap.NET.Common.Models.PushNotification;
using IOBootstrap.NET.Common.MWConnector;
using IOBootstrap.NET.PushNotificationFunctionHelper.Common.Models;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.PushNotificationFunctionHelper.Utilities
{
    public static class PushSenderUtilities
    {
        public static void Run(string projectDirectory, ILogger log)
        {
            string configFilePath = Path.Combine(projectDirectory, "../config.json");
            string configurationJson = System.IO.File.ReadAllText(configFilePath);

            ConfigurationModel configuration = JsonSerializer.Deserialize<ConfigurationModel>(configurationJson);

            FirebaseUtils firebase = new FirebaseUtils(configuration.IOFirebaseApiUrl, configuration.IOFirebaseToken, log);

            string apnsFilePath = Path.Combine(projectDirectory, "../" + configuration.IOAPNSKeyFilePath);
            APNSHttpServiceUtils apns = new APNSHttpServiceUtils(configuration.IOAPNSApiURL, configuration.IOAPNSAuthKeyID, configuration.IOAPNSBundleID, apnsFilePath, configuration.IOAPNSTeamID, log);
            IOMWConnector mwConnector = new IOMWConnector(log, configuration.IOMWEncryptionKey, configuration.IOMWEncryptionIV, configuration.IOMWAuthorizationKey, configuration.IOMWURL);

            string pushNotificationFunctionControllerName = configuration.IOFunctionsPushNotificationControllerName;

            IList<PushNotificationMessageModel> pendingMessages = GetPendingMessages(mwConnector, pushNotificationFunctionControllerName);
            SendNotifications(pendingMessages, mwConnector, pushNotificationFunctionControllerName, log, firebase, apns);
        }

        public static IList<PushNotificationMessageModel> GetPendingMessages(IOMWConnector connector, string controllerName)
        {
            string path = controllerName + "/PendingMessages";
            IOMWListResponseModel<PushNotificationMessageModel> response = connector.Get<IOMWListResponseModel<PushNotificationMessageModel>>(path, new IOMWFindRequestModel());
            connector.HandleResponse<IOMWListResponseModel<PushNotificationMessageModel>>(response, code => {
                throw new IOMWConnectionException();
            });

            return response.Items;
        }

        public static IList<PushNotificationDevicesModel> PrepareDevices(IOMWConnector connector, string controllerName, DeviceTypes deviceType, int messageID, int? clientID)
        {
            string path = controllerName + "/GetDevices";
            IOMWListResponseModel<PushNotificationDevicesModel> response = connector.Get<IOMWListResponseModel<PushNotificationDevicesModel>>(path, new IOMWPushNotificationDevicesRequestModel()
            {
                DeviceType = deviceType,
                MessageId = messageID,
                ClientId = clientID
            });

            return (response != null) ? response.Items : new List<PushNotificationDevicesModel>();
        }

        public static void SendNotifications(IList<PushNotificationMessageModel> pushNotificationMessages, IOMWConnector connector, string controllerName, ILogger log, FirebaseUtils firebase, APNSHttpServiceUtils apnsUtils)
        {
            // Loop throught messages
            foreach(PushNotificationMessageModel message in pushNotificationMessages)
            {
                // Check notification is not for single device
                if (message.PushNotificationDeviceID.DeviceType == DeviceTypes.Generic)
                {
                    // Obtain client id
                    int? clientId = (message.Client == null) ? new int?() : message.Client.Id;

                    // Obtain firebase devices
                    IList<PushNotificationDevicesModel> firebaseDevices = PrepareDevices(connector, controllerName, DeviceTypes.Android, message.ID, clientId);
                    SendNotificationToAllFirebaseDevices(message, firebaseDevices, log, firebase, connector, controllerName);

                    // Obtain apns devices
                    IList<PushNotificationDevicesModel> apnsDevices = PrepareDevices(connector, controllerName, DeviceTypes.iOS, message.ID, clientId);
                    SendNotificationToAllAPNSDevices(message, apnsDevices, log, apnsUtils, connector, controllerName);

                    if (firebaseDevices.Count == 0 && apnsDevices.Count == 0)
                    {
                        SetMessageToSended(message, connector, controllerName);
                    }
                }
                else 
                {
                    IList<PushNotificationDevicesModel> devices = new List<PushNotificationDevicesModel>();
                    devices.Add(new PushNotificationDevicesModel()
                    {
                        ID = message.PushNotificationDeviceID.ID,
                        BadgeCount = message.PushNotificationDeviceID.BadgeCount,
                        DeviceId = message.PushNotificationDeviceID.DeviceId,
                        DeviceToken = message.PushNotificationDeviceID.DeviceToken,
                        DeviceType = message.PushNotificationDeviceID.DeviceType,
                    });

                    if (message.PushNotificationDeviceID.DeviceType == DeviceTypes.Android) 
                    {
                        // Send firebase message
                        SendNotificationToAllFirebaseDevices(message, devices, log, firebase, connector, controllerName);
                    }
                    else if (message.PushNotificationDeviceID.DeviceType == DeviceTypes.iOS)
                    {
                        // Send apns message
                        SendNotificationToAllAPNSDevices(message, devices, log, apnsUtils, connector, controllerName);
                    }

                    // Log call 
                    log.LogDebug("Single device found.");
                    SetMessageToSended(message, connector, controllerName);
                }
            }
        }

        public static void SendNotificationToAllFirebaseDevices(PushNotificationMessageModel message, IList<PushNotificationDevicesModel> firebaseDevices, ILogger log, FirebaseUtils firebase, IOMWConnector connector, string controllerName)
        {
            // Log call 
            log.LogDebug("Firebase devices found size of {0}", firebaseDevices.Count);
            IList<PushNotificationDevicesModel> invalidDevices = new List<PushNotificationDevicesModel>();
            IList<PushNotificationDeliveredMessageModel> deliveredMessages = new List<PushNotificationDeliveredMessageModel>();

            // Loop throught devices
            foreach (PushNotificationDevicesModel pushNotification in firebaseDevices)
            {
                FirebaseModel firebaseModel = new FirebaseModel(pushNotification.DeviceToken,
                                                                message.NotificationTitle,
                                                                message.NotificationMessage,
                                                                message.NotificationCategory,
                                                                message.ID,
                                                                message.NotificationData,
                                                                pushNotification.BadgeCount);
                FirebaseUtils.FirebaseUtilsMessageTypes response = firebase.SendNotifications(firebaseModel);

                if (response == FirebaseUtils.FirebaseUtilsMessageTypes.Success)
                {
                    PushNotificationDeliveredMessageModel deliveredMessage = new PushNotificationDeliveredMessageModel()
                    {
                        PushNotificationID = pushNotification.ID,
                        PushNotificationMessageID = message.ID
                    };
                    deliveredMessages.Add(deliveredMessage);
                }
                else
                {
                    invalidDevices.Add(pushNotification);
                }
            }

            UpdateDeliveredMessages(invalidDevices, deliveredMessages, connector, controllerName);
        }

        private static void SendNotificationToAllAPNSDevices(PushNotificationMessageModel message, IList<PushNotificationDevicesModel> apnsDevices, ILogger log, APNSHttpServiceUtils apns, IOMWConnector connector, string controllerName)
        {
            // Log call 
            log.LogDebug("APNS devices found size of {0}", apnsDevices.Count);
            IList<PushNotificationDevicesModel> invalidDevices = new List<PushNotificationDevicesModel>();
            IList<PushNotificationDeliveredMessageModel> deliveredMessages = new List<PushNotificationDeliveredMessageModel>();

            // Loop throught devices
            foreach (PushNotificationDevicesModel pushNotification in apnsDevices)
            {
                APNSSendPayloadModel sendPayloadModel = new APNSSendPayloadModel(message.NotificationTitle, 
                                                                                message.NotificationMessage, 
                                                                                pushNotification.BadgeCount, 
                                                                                pushNotification.DeviceToken, 
                                                                                message.NotificationData,
                                                                                message.NotificationCategory);
                APNSHttpServiceUtils.APNSHttpServiceUtilsMessageTypes response = apns.SendNotifications(sendPayloadModel);

                if (response == APNSHttpServiceUtils.APNSHttpServiceUtilsMessageTypes.Success)
                {
                    PushNotificationDeliveredMessageModel deliveredMessage = new PushNotificationDeliveredMessageModel()
                    {
                        PushNotificationID = pushNotification.ID,
                        PushNotificationMessageID = message.ID
                    };
                    deliveredMessages.Add(deliveredMessage);
                    log.LogDebug("Notification sended to device {0} badge {1}", pushNotification.DeviceToken, pushNotification.BadgeCount);
                }
                else if (response == APNSHttpServiceUtils.APNSHttpServiceUtilsMessageTypes.DeviceNotFound)
                {
                    invalidDevices.Add(pushNotification);
                }
            }

            UpdateDeliveredMessages(invalidDevices, deliveredMessages, connector, controllerName);
        }

        private static void UpdateDeliveredMessages(IList<PushNotificationDevicesModel> invalidDevices, IList<PushNotificationDeliveredMessageModel> deliveredMessages, IOMWConnector connector, string controllerName)
        {
            string path = controllerName + "/UpdateDeliveredMessages";
            IOResponseModel response = connector.Get<IOResponseModel>(path, new IOMWUpdatePushNotificationDeliveredMessages()
            {
                InvalidDevices = invalidDevices,
                DeliveredMessages = deliveredMessages
            });
        }

        private static void SetMessageToSended(PushNotificationMessageModel message, IOMWConnector connector, string controllerName)
        {
            string path = controllerName + "/SetMessageSended";
            IOResponseModel response = connector.Get<IOResponseModel>(path, new IOMWFindRequestModel()
            {
                ID = message.ID
            });
        }
    }
}
