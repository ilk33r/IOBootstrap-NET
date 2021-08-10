using System;
using System.Collections.Generic;
using System.Linq;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Models.Clients;
using IOBootstrap.NET.Common.Models.PushNotification;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace IOBootstrap.NET.BackOffice.PushNotification.ViewModels
{
    public class IOPushNotificationBackOfficeViewModel<TDBContext> : IOBackOfficeViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
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
            PushNotificationMessageEntity messageEntity = DatabaseContext.PushNotificationMessages.Find(messageId);

            // Check message entity
            if (messageEntity == null)
            {
                return;
            }

            // Set message completed
            if (messageEntity.IsCompleted == 0) 
            {
                messageEntity.IsCompleted = 1;
                DatabaseContext.Update(messageEntity);
                DatabaseContext.SaveChanges();
            }

            // Obtain delivered messages
            var deliveredMessages = DatabaseContext.PushNotificationDeliveredMessages
                                                   .Where((arg) => arg.PushNotificationMessage == messageEntity);

            // Loop throught delivered messages
            foreach (PushNotificationDeliveredMessagesEntity deliveredMessage in deliveredMessages) 
            {
                DatabaseContext.Remove(deliveredMessage);
            }

            if (deliveredMessages.Count() > 0)
            {
                DatabaseContext.SaveChanges();
            }

            DatabaseContext.Remove(messageEntity);
            DatabaseContext.SaveChanges();
        }

        public List<PushNotificationMessageModel> ListMessages()
        {
            // Obtain push notification entity
            IQueryable<PushNotificationMessageEntity> pushNotificationsEntities = DatabaseContext.PushNotificationMessages
                                                                                                    .Include(p => p.Client)
                                                                                                    .OrderByDescending(p => p.ID);

            // Create messages array
            List<PushNotificationMessageModel> messages = new List<PushNotificationMessageModel>();

            // Loop throught entities
            foreach (PushNotificationMessageEntity message in pushNotificationsEntities)
            {
                IOClientInfoModel clientInfoModel = null;
                
                if (message.Client != null)
                {
                    clientInfoModel = new IOClientInfoModel(message.Client.ID, 
                                                            message.Client.ClientId,
                                                            message.Client.ClientSecret,
                                                            message.Client.ClientDescription,
                                                            message.Client.IsEnabled,
                                                            message.Client.RequestCount,
                                                            message.Client.MaxRequestCount);
                }

                // Create push notification model
                PushNotificationMessageModel pushNotificationModel = new PushNotificationMessageModel()
                {
                    ID = message.ID,
                    Client = clientInfoModel,
                    DeviceType = message.DeviceType,
                    NotificationCategory = message.NotificationCategory,
                    NotificationData = message.NotificationData,
                    NotificationDate = message.NotificationDate,
                    NotificationTitle = message.NotificationTitle,
                    NotificationMessage = message.NotificationMessage,
                    IsCompleted = message.IsCompleted
                };

                // Add model to devices array
                messages.Add(pushNotificationModel);
            }

            // Return devices
            return messages;
        }

        public List<PushNotificationModel> ListTokens(int start, int limit)
        {
            // Obtain push notification devices
            List<PushNotificationModel> devices = DatabaseContext.PushNotifications
                                                                                .Select(pn => new PushNotificationModel()
                                                                                {
                                                                                    ID = pn.ID,
                                                                                    AppBuildNumber = pn.AppBuildNumber,
                                                                                    AppBundleId = pn.AppBundleId,
                                                                                    AppVersion = pn.AppVersion,
                                                                                    BadgeCount = pn.BadgeCount,
                                                                                    DeviceId = pn.DeviceId,
                                                                                    DeviceName = pn.DeviceName,
                                                                                    DeviceToken = pn.DeviceToken,
                                                                                    DeviceType = (DeviceTypes)pn.DeviceType,
                                                                                    LastUpdateTime = pn.LastUpdateTime
                                                                                })
                                                                                .Skip(start)
                                                                                .Take(limit)
                                                                                .OrderBy(arg => arg.ID)
                                                                                .ToList();

            // Return devices
            return devices;
        }

        public void SendNotifications(int? clientId,
                                      DeviceTypes deviceType,
                                      string notificationCategory,
                                      string notificationData,
                                      string notificationMessage,
                                      string notificationTitle)
        {
            // Obtain client
            IOClientsEntity clientsEntity = null;
            
            if (clientId != null)
            {
                clientsEntity = DatabaseContext.Clients.Find(clientId);
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
            DatabaseContext.Add(pushNotificationMessageEntity);
            DatabaseContext.SaveChanges();
        }

        #endregion
    }
}
