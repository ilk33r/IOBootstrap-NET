using System;
using System.Collections.Generic;
using IOBootstrap.NET.Batch.Application;
using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Models.Firebase;
using IOBootstrap.NET.Core.Firebase;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Batch.PushNotifications
{
    public class IOSendFirebaseNotifications<TDBContext> : IOBatchClass<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {
        #region Properties

        protected FirebaseUtils Firebase;

        #endregion

        #region Initialization Methods

        protected IOSendFirebaseNotifications(string environment, IConfiguration configuration, string configurationPath, TDBContext databaseContext, ILogger<IOLoggerType> logger) : base(environment, configuration, configurationPath, databaseContext, logger)
        {
            // Obtain firebase configuration
            string firebaseApiUrl = Configuration.GetValue<string>(IOConfigurationConstants.FirebaseApiUrl);
            string firebaseToken = Configuration.GetValue<string>(IOConfigurationConstants.FirebaseToken);

            Firebase = new FirebaseUtils(firebaseApiUrl, firebaseToken, logger);
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

            // Obtain firebase devices
            IOCacheObject firebaseDevicesCacheObject = IOCache.GetCachedObject(IOCacheKeys.PushNotificationFirebaseDevices);
            if (firebaseDevicesCacheObject == null)
            {
                return;
            }

            IList<PushNotificationEntity> firebaseDevices = (IList<PushNotificationEntity>)firebaseDevicesCacheObject.Value;
            if (firebaseDevices == null)
            {
                return;
            }

            // Send notification to devices
            SendPushNotificationToFirebaseDevices(pushNotificationMessage, firebaseDevices);
        }

        #endregion

        #region Helper Methods

        private void SendPushNotificationToFirebaseDevices(PushNotificationMessageEntity pushNotificationMessage, IList<PushNotificationEntity> firebaseDevices)
        {
            IList<PushNotificationEntity> invalidDevices = new List<PushNotificationEntity>();

            // Loop throught devices
            foreach (PushNotificationEntity pushNotification in firebaseDevices)
            {
                FirebaseModel firebaseModel = new FirebaseModel(pushNotification.DeviceToken, pushNotificationMessage.NotificationTitle, pushNotificationMessage.NotificationMessage, pushNotificationMessage.NotificationCategory, pushNotificationMessage.ID);
                FirebaseUtils.FirebaseUtilsMessageTypes response = Firebase.SendNotifications(firebaseModel);

                if (response == FirebaseUtils.FirebaseUtilsMessageTypes.Success)
                {
                    PushNotificationDeliveredMessagesEntity deliveredMessageEntity = new PushNotificationDeliveredMessagesEntity() 
                    {
                        PushNotification = pushNotification,
                        PushNotificationMessage = pushNotificationMessage
                    };
                    DatabaseContext.Add(deliveredMessageEntity);
                }
                else if (response == FirebaseUtils.FirebaseUtilsMessageTypes.DeviceNotFound)
                {
                    invalidDevices.Add(pushNotification);
                }
            }

            DatabaseContext.SaveChanges();
            DeleteInvalidDevices(invalidDevices);
        }

        private void DeleteInvalidDevices(IList<PushNotificationEntity> invalidDevices)
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
