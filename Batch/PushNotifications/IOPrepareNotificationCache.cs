using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IOBootstrap.NET.Batch.Application;
using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Batch.PushNotifications
{
    public abstract class IOPrepareNotificationCache<TDBContext> : IOBatchClass<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Initialization Methods

        protected IOPrepareNotificationCache(string environment, IConfiguration configuration, string configurationPath, TDBContext databaseContext, ILogger<IOLoggerType> logger) : base(environment, configuration, configurationPath, databaseContext, logger)
        {
        }

        #endregion

        #region Batch Methods

        public override void Run()
        {
            base.Run();

            // Check batch is working
            if (CheckDifferentBatchIsWorking())
            {
                // Log call
                Logger.LogDebug("A different operation is running");
                return;
            }

            // Obtain message for sending 
            PushNotificationMessageEntity pushNotificationMessage = GetPushNotificationMessage();

            if (pushNotificationMessage != null) 
            {
                // Create lock file
                CreateLockFile();

                // Log call
                Logger.LogDebug("Sending push notification message for id {0}", pushNotificationMessage.ID);

                // Cache message
                IOCacheObject pushNotificationMessageCache = new IOCacheObject(IOCacheKeys.PushNotificationMessage, pushNotificationMessage, 0);
                IOCache.CacheObject(pushNotificationMessageCache);

                // Obtain client id
                int? clientId = (pushNotificationMessage.Client == null) ? new int?() : pushNotificationMessage.Client.ID;

                // Obtain firebase devices
                IList<PushNotificationEntity> firebaseDevices = PrepareDevices(DeviceTypes.Android, pushNotificationMessage.ID, clientId);
                if (firebaseDevices == null)
                {
                    firebaseDevices = new List<PushNotificationEntity>();
                }

                // Cache firebase devices
                IOCacheObject firebaseDevicesCacheObject = new IOCacheObject(IOCacheKeys.PushNotificationFirebaseDevices, firebaseDevices, 0);
                IOCache.CacheObject(firebaseDevicesCacheObject);

                // Log call 
                Logger.LogDebug("Firebase devices found size of {0}", firebaseDevices.Count());

                // Obtain apns devices
                IList<PushNotificationEntity> apnsDevices = PrepareDevices(DeviceTypes.iOS, pushNotificationMessage.ID, clientId);
                if (apnsDevices == null)
                {
                    apnsDevices = new List<PushNotificationEntity>();
                }

                // Cache apns devices
                IOCacheObject apnsDevicesCacheObject = new IOCacheObject(IOCacheKeys.PushNotificationAPNSDevices, apnsDevices, 0);
                IOCache.CacheObject(apnsDevicesCacheObject);

                // Log call 
                Logger.LogDebug("APNS devices found size of {0}", apnsDevices.Count());
            }
            else 
            {
                IOCache.InvalidateCache(IOCacheKeys.PushNotificationMessage);
                IOCache.InvalidateCache(IOCacheKeys.PushNotificationFirebaseDevices);
            }
        }

        #endregion

        #region Helper Methods

        private bool CheckDifferentBatchIsWorking()
        {
            string tempPath = Path.GetTempPath();
            string lockFile = Path.Combine(tempPath, IOPushNotificationBatchConstants.BatchLockFileName);
            if (File.Exists(lockFile))
            {
                bool isWorking = true;
                using (StreamReader reader = new StreamReader(lockFile))
                {
                    string timeIntervalString = reader.ReadLine();
                    if (timeIntervalString != null)
                    {
                        long timeInterval = Int64.Parse(timeIntervalString);
                        long currentDateTime = DateTimeOffset.Now.ToUnixTimeSeconds();
                        if (timeInterval + IOPushNotificationBatchConstants.BatchTimeoutDuration < currentDateTime)
                        {
                            isWorking = false;
                            File.Delete(lockFile);
                        }
                    }
                }

                return isWorking;
            }

            return false;
        }

        private void CreateLockFile()
        {
            string tempPath = Path.GetTempPath();
            string lockFile = Path.Combine(tempPath, IOPushNotificationBatchConstants.BatchLockFileName);
            long currentDateTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            using (StreamWriter writer = new StreamWriter(lockFile))
            {
                writer.WriteLine(currentDateTime.ToString());
            }
        }

        private PushNotificationMessageEntity GetPushNotificationMessage()
        {
            IQueryable<PushNotificationMessageEntity> pushNotificationMessages = DatabaseContext.PushNotificationMessages
                                                                                                    .Where(pushNotificationMessages => pushNotificationMessages.IsCompleted == 0)
                                                                                                    .Include(pushNotificationMessages => pushNotificationMessages.Client)
                                                                                                    .OrderByDescending(pushNotificationMessages => pushNotificationMessages.NotificationDate)
                                                                                                    .Take(1);

            if (pushNotificationMessages != null && pushNotificationMessages.Count() > 0)
            {
                return pushNotificationMessages.First();
            }

            return null;
        }

        private IList<PushNotificationEntity> PrepareDevices(DeviceTypes deviceType, int messageId, int? clientId)
        {
            IQueryable<PushNotificationEntity> pushNotifications = DatabaseContext.PushNotifications
                                                                                    .Include(pushNotifications => pushNotifications.Client)
                                                                                    .Include(pushNotifications => pushNotifications.DeliveredMessages)
                                                                                    .ThenInclude(pushNotificationDeliveredMessages => pushNotificationDeliveredMessages.PushNotificationMessage)
                                                                                    .Where(pushNotifications => pushNotifications.DeviceType == deviceType)
                                                                                    .Where(pn => pn.DeliveredMessages.All(dm => dm.PushNotificationMessage.ID != messageId))
                                                                                    .Take(IOPushNotificationBatchConstants.BatchEntityCount);

            if (clientId != null)
            {
                pushNotifications = pushNotifications.Where(pn => pn.Client.ID == (int)clientId);
            }

            return pushNotifications.ToList();
        }

        #endregion
    }
}
