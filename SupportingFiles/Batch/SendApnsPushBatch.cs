using IOBootstrap.NET.Application;
using IOBootstrap.NET.Core.APNS.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Toqmak.Api.Common.Enumerations;
using Toqmak.Api.Core.Database;
using Toqmak.Api.WebApi.PushNotification.Entities.PushNotification;
using Toqmak.Api.WebApi.PushNotification.Entities;
using IOBootstrap.NET.Core.APNS.Utils.Models;

namespace Toqmak.Batch.SendPushBatch
{
    public class SendApnsPushBatch : IOBatchClass<ToqmakDatabaseContext>
    {

        private const int SEND_TOKEN_PER_ENTITY = 20;

        #region Initialization Methods

        public SendApnsPushBatch(bool isDevelopment, 
                                 IConfiguration configuration, 
                                 string configurationPath, 
                                 ToqmakDatabaseContext databaseContext, 
                                 ILogger logger)
            : base(isDevelopment, configuration, configurationPath, databaseContext, logger)
        {
        }

        #endregion

        #region Batch Methods

        public override void Run()
        {
            base.Run();

            // Obtain push notification messages
            var pushNotificationsEntities = _databaseContext.PushNotificationMessageEntity
                                                .Where((arg) => arg.DeviceType == (int)DeviceTypes.iOS || arg.DeviceType == (int)DeviceTypes.Generic)
                                                .Where((arg) => arg.IsCompleted != (int)DeviceTypes.Generic && arg.IsCompleted != (int)DeviceTypes.iOS)
                                                .OrderByDescending((arg) => arg.NotificationDate);

            // Loop throught messages
            foreach (PushNotificationMessageEntity messageEntity in pushNotificationsEntities)
            {
                // Obtain devices for messages
                PushNotificationEntity[] devicesForMessage = this.DevicesForMessage(messageEntity);

                // Send message to devices
                this.SendEntitiesToDevices(messageEntity, devicesForMessage);

                // Obtain completed value
                if (messageEntity.IsCompleted == (int)DeviceTypes.Android) 
                {
                    messageEntity.IsCompleted = (int)DeviceTypes.Generic;
                } else {
                    messageEntity.IsCompleted = (int)DeviceTypes.iOS;
                }

                // Update message completed
                _databaseContext.Update(messageEntity);
            }

            // Commit transaction
            _databaseContext.SaveChangesAsync();
        }

        #endregion

        #region Helper Methods

        private PushNotificationEntity[] DevicesForMessage(PushNotificationMessageEntity messageEntity)
        {
            // Obtain push notification devices
            var devicesForMessage = _databaseContext.PushNotifications
                                        .Where((arg) => arg.DeviceType == (int)DeviceTypes.iOS)
                                        .Where((arg) => _databaseContext.PushNotificationDeliveredMessagesEntity
                                            .Where((arg2) => arg2.pushNotificationEntity.ID == arg.ID)
                                            .Where((arg2) => arg2.pushNotificationMessageEntity.ID == messageEntity.ID)
                                            .Count() == 0)
                                        .OrderBy((arg) => arg.LastUpdateTime);
            return devicesForMessage.ToArray();
        }

        private void SendEntitiesToDevices(PushNotificationMessageEntity messageEntity,
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
                    pushNotificationEntity = entity,
                    pushNotificationMessageEntity = messageEntity
                };

                // Add delivered message entity
                _databaseContext.AddAsync(deliveredMessageEntity);

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

        private void SendPushMessages(List<APNSSendPayloadModel> pushNotificationModels) 
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
    }

    #endregion
}
