using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using IOBootstrap.NET.Batch.Application;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Core.APNS.Utils;
using IOBootstrap.NET.Core.APNS.Utils.Models;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.WebApi.PushNotification.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Batch.PushNotifications
{
    public abstract class IOSendPushNotificationBatch<TDBContext> : IOBatchClass<TDBContext>
        where TDBContext : IODatabaseContext<TDBContext>
    {

        private const int SEND_TOKEN_PER_ENTITY = 20;

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
                                                .Where((arg) => arg.IsCompleted != 0)
                                                .OrderByDescending((arg) => arg.NotificationDate);

            // Loop throught messages
            foreach (PushNotificationMessageEntity messageEntity in pushNotificationsEntities)
            {
                // Obtain devices for messages
                PushNotificationEntity[] devicesForMessage = this.DevicesForMessage(messageEntity);

                // Send message to devices
                this.SendEntitiesToDevices(messageEntity, devicesForMessage);

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
                                                    .Where((arg) => arg.DeviceType == (int)DeviceTypes.iOS || arg.DeviceType == (int)DeviceTypes.Android || arg.DeviceType == (int)DeviceTypes.Generic)
                                                    .Where((arg) => _databaseContext.PushNotificationDeliveredMessages
                                                           .Where((arg2) => arg2.PushNotification.ID == arg.ID)
                                                           .Where((arg2) => arg2.PushNotificationMessage.ID == messageEntity.ID)
                                                           .Count() == 0)
                                                    .OrderBy((arg) => arg.LastUpdateTime);
            return devicesForMessage.ToArray();
        }

        public virtual void SendEntitiesToDevices(PushNotificationMessageEntity messageEntity,
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
                apnsPushNotificationModels.Append(apnsPayloadModel);

                // Check index is greater than max entity count
                if (i >= SEND_TOKEN_PER_ENTITY)
                {
                    // Send push messages
                    this.SendPushMessages(apnsPushNotificationModels);

                    // Sleep thread
                    Thread.Sleep(1000);

                    // Send entities to devices
                    this.SendEntitiesToDevices(messageEntity, pushNotificationsEntities, i);

                    // Break the loop
                    break;
                }
            }

            // Sleep thread
            Thread.Sleep(1000);

            // Send push messages
            this.SendPushMessages(apnsPushNotificationModels);
        }

        public virtual void SendPushMessages(List<APNSSendPayloadModel> pushNotificationModels)
        {
            // Obtain apns configuration
            string apnsHost = _configuration.GetValue<string>("IOAPNSHost");
            int apnsPort = _configuration.GetValue<int>("IOAPNSPort");
            string certFile = _configurationPath + _configuration.GetValue<string>("IOAPNSCertificatePath");
            string certPassword = _configuration.GetValue<string>("IOAPNSCertificatePassword");

            // Create apns utils
            APNSUtils apnsUtils = new APNSUtils(apnsHost, apnsPort, certFile, certPassword, _logger);
            apnsUtils.SendNotifications(pushNotificationModels);
        }

        #endregion
    }
}
