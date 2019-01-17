using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using IOBootstrap.NET.Batch.Application;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Core.APNS.Utils;
using IOBootstrap.NET.Core.APNS.Utils.Models;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.Core.Firebase.Models;
using IOBootstrap.NET.WebApi.PushNotification.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Batch.PushNotifications
{
    public abstract class IOSendPushNotificationBatch<TDBContext> : IOBatchClass<TDBContext>
        where TDBContext : IODatabaseContext<TDBContext>
    {

        public const int SEND_TOKEN_PER_ENTITY = 25;

        #region Initialization Methods

        public IOSendPushNotificationBatch(bool isDevelopment,
                                           IConfiguration configuration,
                                           string configurationPath,
                                           TDBContext databaseContext,
                                           ILogger logger) : base(isDevelopment, configuration, configurationPath, databaseContext, logger)
        {
        }

        #endregion

        #region Batch Methods

        public override void Run()
        {
            base.Run();

            // Obtain push notification messages
            var pushNotificationsEntities = _databaseContext.PushNotificationMessages
                                                            .Where((arg) => arg.IsCompleted == 0)
                                                            .Include(d => d.Client)
                                                            .OrderByDescending((arg) => arg.NotificationDate);

            // Loop throught messages
            foreach (PushNotificationMessageEntity messageEntity in pushNotificationsEntities)
            {
                // Obtain devices for messages
                PushNotificationEntity[] devicesForMessage = this.DevicesForMessage(messageEntity);

                // Send message to devices
                this.SendEntitiesToiOSDevices(messageEntity, devicesForMessage);
                this.SendEntitiesToAndroidDevices(messageEntity, devicesForMessage);

                // Set completed status
                messageEntity.IsCompleted = 1;

                // Update message completed
                _databaseContext.Update(messageEntity);
            }

            // Commit transaction
            _databaseContext.SaveChanges();
        }

        #endregion

        #region Helper Methods

        public virtual PushNotificationEntity[] DevicesForMessage(PushNotificationMessageEntity messageEntity)
        {
            // Obtain push notification devices
            var devicesForMessage = _databaseContext.PushNotifications
                                                    .Where((arg) => arg.Client == messageEntity.Client 
                                                           && (arg.DeviceType == (int)DeviceTypes.iOS || arg.DeviceType == (int)DeviceTypes.Android || arg.DeviceType == (int)DeviceTypes.Generic))
                                                    .Where((arg) => _databaseContext.PushNotificationDeliveredMessages
                                                           .Where((arg2) => arg2.PushNotification.ID == arg.ID)
                                                           .Where((arg2) => arg2.PushNotificationMessage.ID == messageEntity.ID)
                                                           .Count() == 0)
                                                    .OrderBy((arg) => arg.LastUpdateTime);
            return devicesForMessage.ToArray();
        }

        public virtual void SendEntitiesToAndroidDevices(PushNotificationMessageEntity messageEntity,
                                                         PushNotificationEntity[] pushNotificationsEntities,
                                                         int startIndex = 0)
        {
            // Create push notifications list
            List<string> registrationIds = new List<string>();
            FirebaseDataModel firebaseData = new FirebaseDataModel(messageEntity.NotificationTitle, messageEntity.NotificationMessage, "other", messageEntity.ID);

            // Loop throught push notification entities
            for (int i = startIndex; i < pushNotificationsEntities.Count(); i++)
            {
                // Obtain push notification entity
                PushNotificationEntity entity = pushNotificationsEntities[i];

                if (entity.DeviceType != (int)DeviceTypes.Android)
                {
                    continue;
                }

                // Update badge count in entity
                entity.BadgeCount = entity.BadgeCount + 1;
                entity.LastUpdateTime = DateTime.UtcNow;

                // Update entity
                _databaseContext.Update(entity);

                // Create delivered messages entity
                PushNotificationDeliveredMessagesEntity deliveredMessageEntity = new PushNotificationDeliveredMessagesEntity
                {
                    // Setup model properties
                    PushNotification = entity,
                    PushNotificationMessage = messageEntity
                };

                // Add delivered message entity
                _databaseContext.Add(deliveredMessageEntity);

                // Append token to list
                registrationIds.Add(entity.DeviceToken);

                // Check index is greater than max entity count
                if (i >= SEND_TOKEN_PER_ENTITY)
                {
                    // Send push messages
                    this.SendAndroidPushMessages(new FirebaseModel(registrationIds, firebaseData), (bool status, FirebaseResponseModel responseObject) => {
                        // Sleep thread
                        Thread.Sleep(100);

                        // Send entities to devices
                        this.SendEntitiesToiOSDevices(messageEntity, pushNotificationsEntities, i);
                    });

                    // Break the loop
                    break;
                }
            }

            // Sleep thread
            Thread.Sleep(100);

            // Send push messages
            this.SendAndroidPushMessages(new FirebaseModel(registrationIds, firebaseData), (bool status, FirebaseResponseModel responseObject) => {
            });
        }

        public virtual void SendEntitiesToiOSDevices(PushNotificationMessageEntity messageEntity,
                                                  PushNotificationEntity[] pushNotificationsEntities,
                                                  int startIndex = 0)
        {
            // Create push notifications list
            List<APNSSendPayloadModel> apnsPushNotificationModels = new List<APNSSendPayloadModel>();

            // Loop throught push notification entities
            for (int i = startIndex; i < pushNotificationsEntities.Count(); i++)
            {
                // Obtain push notification entity
                PushNotificationEntity entity = pushNotificationsEntities[i];

                // Create apns payload
                APNSSendPayloadModel apnsPayloadModel = new APNSSendPayloadModel(entity.BadgeCount,
                                                                                 messageEntity.NotificationMessage,
                                                                                 messageEntity.NotificationTitle,
                                                                                 entity.DeviceToken);

                if (entity.DeviceType != (int)DeviceTypes.iOS)
                {
                    continue;
                }

                // Update badge count in entity
                entity.BadgeCount = entity.BadgeCount + 1;
                entity.LastUpdateTime = DateTime.UtcNow;

                // Update entity
                _databaseContext.Update(entity);

                // Create delivered messages entity
                PushNotificationDeliveredMessagesEntity deliveredMessageEntity = new PushNotificationDeliveredMessagesEntity
                {
                    // Setup model properties
                    PushNotification = entity,
                    PushNotificationMessage = messageEntity
                };

                // Add delivered message entity
                _databaseContext.Add(deliveredMessageEntity);

                // Append apns model to list
                apnsPushNotificationModels.Add(apnsPayloadModel);

                // Check index is greater than max entity count
                if (i >= SEND_TOKEN_PER_ENTITY)
                {
                    // Send push messages
                    this.SendApplePushMessages(apnsPushNotificationModels);

                    // Sleep thread
                    Thread.Sleep(100);

                    // Send entities to devices
                    this.SendEntitiesToiOSDevices(messageEntity, pushNotificationsEntities, i);

                    // Break the loop
                    break;
                }
            }

            // Sleep thread
            Thread.Sleep(100);

            // Send push messages
            this.SendApplePushMessages(apnsPushNotificationModels);
        }

        public virtual void SendAndroidPushMessages(FirebaseModel firebaseModel, FirebaseSendNotificationHandler handler)
        {
            // Obtain apns configuration
            string firebaseApiUrl = _configuration.GetValue<string>(IOConfigurationConstants.FirebaseApiUrl);
            string firebaseToken = _configuration.GetValue<string>(IOConfigurationConstants.FirebaseToken);

            // Create apns utils
            FirebaseUtils firebaseUtils = new FirebaseUtils(firebaseApiUrl, firebaseToken, _logger);
            firebaseUtils.SendNotifications(firebaseModel, handler);
        }


        public virtual void SendApplePushMessages(List<APNSSendPayloadModel> pushNotificationModels)
        {
            // Obtain apns configuration
            string apnsHost = _configuration.GetValue<string>(IOConfigurationConstants.APNSHost);
            int apnsPort = _configuration.GetValue<int>(IOConfigurationConstants.APNSPort);
            string certFile = _configurationPath + _configuration.GetValue<string>(IOConfigurationConstants.APNSCertificatePath);
            string certPassword = _configuration.GetValue<string>(IOConfigurationConstants.APNSCertificatePassword);

            // Create apns utils
            APNSUtils apnsUtils = new APNSUtils(apnsHost, apnsPort, certFile, certPassword, _logger);
            apnsUtils.SendNotifications(pushNotificationModels);
        }

        #endregion
    }
}
