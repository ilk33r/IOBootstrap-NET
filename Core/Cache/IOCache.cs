using System;
using System.Collections.Generic;

namespace IOBootstrap.NET.Core.Cache.Utilities
{
    public static class IOCache
    {

        #region Privates

        private static List<IOCacheObject> CachedObjects;
        private static bool IsInitializing = false;

        #endregion

        #region Public Methods

        public static void CacheObject(IOCacheObject cache)
        {
            IOCache.InitializeCache();

            if (IOCache.CacheExists(cache) < 0)
            {
                cache.SetCacheTime();
                IOCache.CachedObjects.Add(cache);
            }
        }

        public static IOCacheObject GetCachedObject(string key)
        {
            IOCache.InitializeCache();
            IOCacheObject cachedObject = IOCache.CachedObjects.Find((obj) => obj.GetKey().Equals(key));
            return cachedObject;
        }

        public static void InvalidateCache(string key)
        {
            IOCache.InitializeCache();
            int index = IOCache.CachedObjects.FindIndex((obj) => obj.GetKey().Equals(key));
            if (index >= 0)
            {
                IOCache.CachedObjects.RemoveAt(index);
            }
        }

        #endregion

        #region Helper Methods

        private static int CacheExists(IOCacheObject cache)
        {
			IOCache.InitializeCache();
            int index = IOCache.CachedObjects.FindIndex((obj) => obj.GetCacheID().Equals(cache.GetCacheID()));
            return index;
        }

        private static void InitializeCache()
        {
            if (IOCache.IsInitializing)
            {
                return;
            }

            IOCache.IsInitializing = true;

            if (IOCache.CachedObjects == null)
            {
                IOCache.CachedObjects = new List<IOCacheObject>();
            }

            DateTimeOffset currentTimeOffset = DateTimeOffset.Now;
            long currentTimeStamp = currentTimeOffset.ToUnixTimeSeconds();
            List<int> forRemoveIndexes = new List<int>();

            foreach (IOCacheObject cache in IOCache.CachedObjects)
            {
                if (cache.GetCacheEndTimeStamp() > 0 && cache.GetCacheEndTimeStamp() < currentTimeStamp)
                {
                    int index = IOCache.CacheExists(cache);
                    if (index >= 0)
                    {
                        forRemoveIndexes.Add(index);
                    }
                }
            }

            foreach (int index in forRemoveIndexes)
            {
                if (index < IOCache.CachedObjects.Count)
                {
                    IOCache.CachedObjects.RemoveAt(index);
                }
            }

            IOCache.IsInitializing = false;
        }

        #endregion
    }
}
