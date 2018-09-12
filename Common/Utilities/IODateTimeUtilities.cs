using System;

namespace IOBootstrap.NET.Common.Utilities
{
    public static class IODateTimeUtilities
    {
        #region Date Time

        public static long UnixTimeFromDate(DateTime dateTime)
        {
            // Calculate unix time stamp
            long epochTicks = new DateTime(1970, 1, 1).Ticks;
            long currentSeconds = ((dateTime.Ticks - epochTicks) / TimeSpan.TicksPerSecond);

            return currentSeconds;
        }

        #endregion
    }
}
