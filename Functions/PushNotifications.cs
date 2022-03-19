using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Functions
{
    public static class PushNotifications
    {
        [FunctionName("PushNotifications")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo timer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
