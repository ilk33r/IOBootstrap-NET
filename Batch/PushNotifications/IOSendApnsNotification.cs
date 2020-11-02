using System;
using System.Collections.Generic;
using System.IO;
using IOBootstrap.NET.Batch.Application;
using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Models.APNS;
using IOBootstrap.NET.Core.APNS;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Batch.PushNotifications
{
    public class IOSendApnsNotification<TDBContext> : IOBatchClass<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {
        #region Properties

        private APNSHttpServiceUtils APNS;

        #endregion

        #region Initialization Methods

        protected IOSendApnsNotification(string environment, IConfiguration configuration, string configurationPath, TDBContext databaseContext, ILogger<IOLoggerType> logger) : base(environment, configuration, configurationPath, databaseContext, logger)
        {
            // Obtain apns configuration
            string apnsApiUrl = Configuration.GetValue<string>(IOConfigurationConstants.APNSApiURL);
            string apnsAuthKeyID = Configuration.GetValue<string>(IOConfigurationConstants.APNSAuthKeyID);
            string apnsBundleID = Configuration.GetValue<string>(IOConfigurationConstants.APNSBundleID);
            string keyFile = Configuration.GetValue<string>(IOConfigurationConstants.APNSKeyFilePath);
            string teamID = Configuration.GetValue<string>(IOConfigurationConstants.APNSTeamID);
            string keyFilePath = Path.Combine(configurationPath, keyFile);

            APNS = new APNSHttpServiceUtils(apnsApiUrl, apnsAuthKeyID, apnsBundleID, keyFilePath, teamID, logger);
        }

        #endregion

        #region Batch Methods

        public override void Run()
        {
            base.Run();

            // Obtain push notification message
            IOCacheObject pushNotificationMessageCache = IOCache.GetCachedObject(IOCacheKeys.PushNotificationMessage);
            if (pushNotificationMessageCache == null)
            {
                return;
            }

            PushNotificationMessageEntity pushNotificationMessage = (PushNotificationMessageEntity)pushNotificationMessageCache.Value;
            if (pushNotificationMessage == null)
            {
                return;
            }

            // Obtain apns devices
            IOCacheObject apnsDevicesCacheObject = IOCache.GetCachedObject(IOCacheKeys.PushNotificationAPNSDevices);
            if (apnsDevicesCacheObject == null)
            {
                return;
            }

            IList<PushNotificationEntity> apnsDevices = (IList<PushNotificationEntity>)apnsDevicesCacheObject.Value;
            if (apnsDevices == null)
            {
                return;
            }

            // Send notification to devices
            SendPushNotificationToAPNSDevices(pushNotificationMessage, apnsDevices);
        }

        #endregion

        #region Helper Methods

        private void SendPushNotificationToAPNSDevices(PushNotificationMessageEntity pushNotificationMessage, IList<PushNotificationEntity> apnsDevices)
        {
            IList<PushNotificationEntity> invalidDevices = new List<PushNotificationEntity>();

            // Loop throught devices
            foreach (PushNotificationEntity pushNotification in apnsDevices)
            {
                APNSSendPayloadModel sendPayloadModel = new APNSSendPayloadModel(pushNotificationMessage.NotificationTitle, pushNotificationMessage.NotificationMessage, 1, pushNotification.DeviceToken);
                APNSHttpServiceUtils.APNSHttpServiceUtilsMessageTypes response = APNS.SendNotifications(sendPayloadModel);

                if (response == APNSHttpServiceUtils.APNSHttpServiceUtilsMessageTypes.Success)
                {
                    PushNotificationDeliveredMessagesEntity deliveredMessageEntity = new PushNotificationDeliveredMessagesEntity() 
                    {
                        PushNotification = pushNotification,
                        PushNotificationMessage = pushNotificationMessage
                    };
                    DatabaseContext.Add(deliveredMessageEntity);
                }
                else if (response == APNSHttpServiceUtils.APNSHttpServiceUtilsMessageTypes.DeviceNotFound)
                {
                    invalidDevices.Add(pushNotification);
                }
            }

            DatabaseContext.SaveChanges();
            DeleteInvalidDevices(invalidDevices);
        }

        protected virtual void DeleteInvalidDevices(IList<PushNotificationEntity> invalidDevices)
        {
            foreach (PushNotificationEntity pushNotification in invalidDevices)
            {
                DatabaseContext.Remove(pushNotification.DeliveredMessages);
                DatabaseContext.Remove(pushNotification);
            }
            
            DatabaseContext.SaveChanges();
        }

        #endregion
    }
}
