using System;
using IOBootstrap.NET.PushNotificationFunctionHelper.Utilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Functions
{
    public static class PushNotifications
    {

        [FunctionName("PushNotifications")]
        public static void Run([TimerTrigger("*/15 * * * * *")]TimerInfo timer, ILogger log)
        {
            PushSenderUtilities.Run(log);
        }
    }
}
