using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.WebApi.BackOffice.ViewModels;
using IOBootstrap.NET.WebApi.PushNotification.Entities;
using IOBootstrap.NET.WebApi.PushNotification.Models;
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

        public void SendNotifications(DeviceTypes deviceType, 
                                      string notificationCategory, 
                                      string notificationData,
                                      string notificationMessage, 
                                      string notificationTitle) {
            // Create push notification message entity
            PushNotificationMessageEntity pushNotificationMessageEntity = new PushNotificationMessageEntity()
            {
                DeviceType = (int)deviceType,
                NotificationCategory = notificationCategory,
                NotificationData = notificationData,
                NotificationMessage = notificationMessage,
                NotificationTitle = notificationTitle,
                NotificationDate = DateTime.UtcNow,
                IsCompleted = (int)DeviceTypes.Unkown
            };

            // Write message to database
            _databaseContext.Add(pushNotificationMessageEntity);
            _databaseContext.SaveChanges();
        }

        #endregion
    }
}
