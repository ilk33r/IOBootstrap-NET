using System;
using System.Collections.Generic;
using System.IO;
using IOBootstrap.NET.Batch.Application;
using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Batch.PushNotifications
{
    public class IOFinalizePushNotificationBatch<TDBContext> : IOBatchClass<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {
        #region Initialization Methods

        protected IOFinalizePushNotificationBatch(string environment, IConfiguration configuration, string configurationPath, TDBContext databaseContext, ILogger<IOLoggerType> logger) : base(environment, configuration, configurationPath, databaseContext, logger)
        {
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

            if (firebaseDevices.Count == 0) 
            {
                pushNotificationMessage.IsCompleted = 1;
                DatabaseContext.Update(pushNotificationMessage);
                DatabaseContext.SaveChanges();
            }

            IOCache.InvalidateCache(IOCacheKeys.PushNotificationMessage);
            IOCache.InvalidateCache(IOCacheKeys.PushNotificationFirebaseDevices);

            string tempPath = Path.GetTempPath();
            string lockFile = Path.Combine(tempPath, IOPushNotificationBatchConstants.BatchLockFileName);
            try
            {
                File.Delete(lockFile);
            }
            catch (Exception e)
            {
                Logger.LogDebug(e.Message);
            }
        }

        #endregion
    }
}
