using IOBootstrap.NET.Batch.Base.Common.Models;
using IOBootstrap.NET.Batch.PushSender.Process;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.Batch.PushSender.Extensions;

public static class IOPushSenderProcessSendMessagesExtension
{

    public static void SendMessages<TConfig, TDBContext>(
        this IOPushSenderProcess<TConfig, TDBContext> input, 
        IList<PushNotificationMessageEntity> pushNotificationMessages
    )
    where TConfig : IOBatchConfigurationModel
    where TDBContext : IODatabaseContext<TDBContext>
    {
        foreach (PushNotificationMessageEntity message in pushNotificationMessages)
        {
            // Obtain client id
            int? clientId = (message.Client == null) ? new int?() : message.Client?.ID;

            // Check notification is not for single device
            if (message.PushNotificationDeviceID?.DeviceType == DeviceTypes.Generic)
            {
                // Obtain firebase devices
                IList<PushNotificationEntity> googleDevices = input.GetDevices(DeviceTypes.AndroidGoogle, message.ID, clientId);
                IList<PushNotificationEntity> invalidDevices = input.SendNotificationToAllFirebaseDevices(message, googleDevices);
                input.DeleteInvalidDevices(invalidDevices);

                // Obtain apns devices
                IList<PushNotificationEntity> appleDevices = input.GetDevices(DeviceTypes.iOS, message.ID, clientId);
                // SendNotificationToAllAPNSDevices(message, apnsDevices, log, apnsUtils, connector, controllerName);

                // if (firebaseDevices.Count == 0 && apnsDevices.Count == 0)
                if (googleDevices.Count == 0)
                {
                    input.SetMessageSended(message);
                }
            }
            else if (message.DeviceType == (int)DeviceTypes.AndroidGoogle)
            {
                // Send firebase message// Obtain firebase devices
                IList<PushNotificationEntity> googleDevices = input.GetDevices(DeviceTypes.AndroidGoogle, message.ID, clientId);
                IList<PushNotificationEntity> invalidDevices = input.SendNotificationToAllFirebaseDevices(message, googleDevices);
                input.DeleteInvalidDevices(invalidDevices);

                if (googleDevices.Count == 0)
                {
                    input.SetMessageSended(message);
                }
            }
            else
            {
                // IList<PushNotificationEntity> devices = new List<PushNotificationEntity>();
                // devices.Add(new PushNotificationDevicesModel()
                // {
                //     ID = message.PushNotificationDeviceID.ID,
                //     BadgeCount = message.PushNotificationDeviceID.BadgeCount,
                //     DeviceId = message.PushNotificationDeviceID.DeviceId,
                //     DeviceToken = message.PushNotificationDeviceID.DeviceToken,
                //     DeviceType = message.PushNotificationDeviceID.DeviceType,
                // });


                // else if (message.PushNotificationDeviceID.DeviceType == DeviceTypes.iOS)
                // {
                //     // Send apns message
                //     SendNotificationToAllAPNSDevices(message, devices, log, apnsUtils, connector, controllerName);
                // }

                // // Log call 
                // log.LogDebug("Single device found.");
                // SetMessageToSended(message, connector, controllerName);
            }
        }
    }

}
