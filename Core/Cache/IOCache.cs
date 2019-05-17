﻿using System;
using System.Collections.Generic;

namespace IOBootstrap.NET.Core.Cache.Utilities
{
    public static class IOCache
    {

        #region Privates

        private static List<IOCacheObject> CachedObjects;

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
            int index = IOCache.CachedObjects.FindIndex((obj) => obj.GetCacheID() == cache.GetCacheID());
            return index;
        }

        private static void InitializeCache()
        {
            if (IOCache.CachedObjects == null)
            {
                IOCache.CachedObjects = new List<IOCacheObject>();
            }

            DateTimeOffset currentTimeOffset = DateTimeOffset.Now;
            long currentTimeStamp = currentTimeOffset.ToUnixTimeSeconds();
            List<int> forRemoveIndexes = new List<int>();

            foreach (IOCacheObject cache in IOCache.CachedObjects)
            {
                if (cache.GetCacheEndTimeStamp() < currentTimeStamp)
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
                IOCache.CachedObjects.RemoveAt(index);
            }
        }

        #endregion
    }
}