using System;
using IOBootstrap.NET.Common.Utilities;

namespace IOBootstrap.NET.Common.Cache
{
    public class IOCacheObject
    {

        #region Privates

        private long CacheInterval;
        private long CacheTime;
        private string CacheID;
        private string Key;

        #endregion

        #region Publics

        public Object Value;

        #endregion

        #region Initialization Methods

        public IOCacheObject(string key, Object objectValue, long cacheInterval)
        {
            CacheInterval = cacheInterval;
            Key = key;
            Value = objectValue;
            CacheID = IORandomUtilities.GenerateGUIDString();
        }

        #endregion

        #region Getters

        public string GetCacheID()
        {
            return CacheID;
        }

        public long GetCacheEndTimeStamp()
        {
            if (CacheInterval == 0)
            {
                return 0;
            }

            return CacheTime + CacheInterval;
        }

        public string GetKey()
        {
            return Key;
        }

        #endregion

        #region Setters

        public void SetCacheTime()
        {
            DateTimeOffset timeOffset = DateTimeOffset.Now;
            CacheTime = timeOffset.ToUnixTimeSeconds();
        }

        #endregion
    }
}
