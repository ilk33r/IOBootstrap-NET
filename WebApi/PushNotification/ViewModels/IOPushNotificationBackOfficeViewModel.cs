using IOBootstrap.NET.Common.Entities.Clients;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.WebApi.PushNotification.Entities;
using IOBootstrap.NET.WebApi.PushNotification.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IOBootstrap.NET.WebApi.PushNotification.ViewModels
{
    public class IOPushNotificationBackOfficeViewModel<TDBContext> : IOBackOfficeViewModel<TDBContext>
        where TDBContext : IODatabaseContext<TDBContext>
    {
        
        #region Initialization Methods

        public IOPushNotificationBackOfficeViewModel()
        {
        }

        #endregion

        #region Back Office Methods

        public void DeleteMessage(int messageId)
        {
            // Obtain message 
            PushNotificationMessageEntity messageEntity = _databaseContext.PushNotificationMessages.Find(messageId);

            // Check message entity
            if (messageEntity == null)
            {
                return;
            }

            // Set message completed
            if (messageEntity.IsCompleted == 0) 
            {
                messageEntity.IsCompleted = 1;
                _databaseContext.Update(messageEntity);
                _databaseContext.SaveChanges();
            }

            // Obtain delivered messages
            var deliveredMessages = _databaseContext.PushNotificationDeliveredMessages
                                                    .Where((arg) => arg.PushNotificationMessage == messageEntity);

            // Loop throught delivered messages
            foreach (PushNotificationDeliveredMessagesEntity deliveredMessage in deliveredMessages) 
            {
                _databaseContext.Remove(deliveredMessage);
            }

            if (deliveredMessages.Count() > 0)
            {
                _databaseContext.SaveChanges();
            }

            _databaseContext.Remove(messageEntity);
            _databaseContext.SaveChanges();
        }

        public List<PushNotificationMessageModel> ListMessages()
        {
            // Obtain push notification entity
            var pushNotificationsEntities = _databaseContext.PushNotificationMessages
                                                            .OrderBy((arg) => arg.ID)
                                                            .Include(p => p.Client);

            // Create messages array
            List<PushNotificationMessageModel> messages = new List<PushNotificationMessageModel>();

            // Loop throught entities
            foreach (PushNotificationMessageEntity message in pushNotificationsEntities)
            {
                // Obtain sended messages
                var sendedMessages = _databaseContext.PushNotificationDeliveredMessages
                                                     .Where((arg) => arg.PushNotificationMessage == message);

                // Create push notification model
                PushNotificationMessageModel pushNotificationModel = new PushNotificationMessageModel()
                {
                    ID = message.ID,
                    Client = message.Client,
                    DeviceType = message.DeviceType,
                    NotificationCategory = message.NotificationCategory,
                    NotificationData = message.NotificationData,
                    NotificationDate = message.NotificationDate,
                    NotificationTitle = message.NotificationTitle,
                    NotificationMessage = message.NotificationMessage,
                    IsCompleted = message.IsCompleted,
                    SendedDevices = sendedMessages.Count()
                };

                // Add model to devices array
                messages.Add(pushNotificationModel);
            }

            // Return devices
            return messages;
        }

        public List<PushNotificationModel> ListTokens(int start, int limit)
        {
            // Obtain push notification entity
            var pushNotificationsEntities = _databaseContext.PushNotifications.OrderBy((arg) => arg.ID);

            // Create devices array
            List<PushNotificationModel> devices = new List<PushNotificationModel>();

            // Create a variables for object count
            int skippedObjectCount = 0;
            int objectCount = 0;

            // Loop throught entities
            foreach (PushNotificationEntity device in pushNotificationsEntities)
            {
                // Check skipped object count is less than skip count
                if (start > skippedObjectCount)
                {
                    // Increase skipped object count
                    skippedObjectCount += 1;

                    // Continue to next object
                    continue;
                }

                // Create push notification model
                PushNotificationModel pushNotificationModel = new PushNotificationModel()
                {
                    ID = device.ID,
                    AppBuildNumber = device.AppBuildNumber,
                    AppBundleId = device.AppBundleId,
                    AppVersion = device.AppVersion,
                    BadgeCount = device.BadgeCount,
                    DeviceId = device.DeviceId,
                    DeviceName = device.DeviceName,
                    DeviceToken = device.DeviceToken,
                    DeviceType = (DeviceTypes)device.DeviceType,
                    LastUpdateTime = device.LastUpdateTime
                };

                // Add model to devices array
                devices.Add(pushNotificationModel);

                // Increase object count
                objectCount += 1;

                // Check object count is greater than limit
                if (objectCount >= limit)
                {
                    // Then break the loop
                    break;
                }
            }

            // Return devices
            return devices;
        }

        public void SendNotifications(int clientId,
                                      DeviceTypes deviceType,
                                      string notificationCategory,
                                      string notificationData,
                                      string notificationMessage,
                                      string notificationTitle)
        {
            // Obtain client
            IOClientsEntity clientsEntity = _databaseContext.Clients.Find(clientId);

            // Check client
            if (clientsEntity == null)
            {
                return;
            }

            // Create push notification message entity
            PushNotificationMessageEntity pushNotificationMessageEntity = new PushNotificationMessageEntity()
            {
                Client = clientsEntity,
                DeviceType = (int)deviceType,
                NotificationCategory = notificationCategory,
                NotificationData = notificationData,
                NotificationMessage = notificationMessage,
                NotificationTitle = notificationTitle,
                NotificationDate = DateTime.UtcNow,
                IsCompleted = 0
            };

            // Write message to database
            _databaseContext.Add(pushNotificationMessageEntity);
            _databaseContext.SaveChanges();
        }

        #endregion
    }
}
