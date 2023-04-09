using System;
using System.Collections.Generic;

namespace IOBootstrap.NET.Common.Cache
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
                IOCache.InvalidateCache(cache.GetKey());
                IOCache.CachedObjects.Add(cache);
            }
        }

        public static void ClearCache() 
        {
            IOCache.CachedObjects = null;
            IOCache.InitializeCache();
        }

        public static IOCacheObject GetCachedObject(string key)
        {
            IOCache.InitializeCache();

            if (IOCache.CachedObjects != null && IOCache.CachedObjects.Count() > 0)
            {
                return IOCache.CachedObjects.Find(obj => obj != null && obj.GetKey().Equals(key));
            }
            
            return null;
        }

        public static void InvalidateCache(string key)
        {
            IOCache.InitializeCache();
            int index = IOCache.CachedObjects.FindIndex(obj => obj != null && obj.GetKey().Equals(key));
            if (index >= 0 && index < IOCache.CachedObjects.Count)
            {
                try {
                    IOCache.CachedObjects.RemoveAt(index);
                } catch {
                }
            }
        }

        #endregion

        #region Helper Methods

        private static int CacheExists(IOCacheObject cache)
        {
			IOCache.InitializeCache();
            int index = IOCache.CachedObjects.FindIndex(obj => obj != null && obj.GetCacheID().Equals(cache.GetCacheID()));
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
                if (cache == null)
                {
                    IOCache.CachedObjects.Remove(cache);
                }
                else if (cache.GetCacheEndTimeStamp() > 0 && cache.GetCacheEndTimeStamp() < currentTimeStamp)
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
