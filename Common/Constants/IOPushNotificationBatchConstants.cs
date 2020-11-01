using System;

namespace IOBootstrap.NET.Common.Constants
{
    public static class IOPushNotificationBatchConstants
    {
        public static int BatchEntityCount = 500;

        public static long BatchTimeoutDuration = 10800; // 3 hours

        public static string BatchLockFileName = "PushNotificationBatch.lock";
    }
}
