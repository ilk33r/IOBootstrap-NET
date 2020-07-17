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
            this.CacheInterval = cacheInterval;
            this.Key = key;
            this.Value = objectValue;
            this.CacheID = IORandomUtilities.GenerateGUIDString();
        }

        #endregion

        #region Getters

        public string GetCacheID()
        {
            return this.CacheID;
        }

        public long GetCacheEndTimeStamp()
        {
            if (this.CacheInterval == 0)
            {
                return 0;
            }

            return this.CacheTime + this.CacheInterval;
        }

        public string GetKey()
        {
            return this.Key;
        }

        #endregion

        #region Setters

        public void SetCacheTime()
        {
            DateTimeOffset timeOffset = DateTimeOffset.Now;
            this.CacheTime = timeOffset.ToUnixTimeSeconds();
        }

        #endregion
    }
}
