using System;
using System.Collections.Generic;

namespace IOBootstrap.NET.Common.Cache
{
    public static class IOCache
    {

        #region Privates

        private static List<IOCacheObject> CachedObjects;
        private static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private static TaskFactory factory = new TaskFactory(
            cancellationTokenSource.Token, 
            TaskCreationOptions.PreferFairness, 
            TaskContinuationOptions.ExecuteSynchronously, 
            TaskScheduler.Default
        );

        #endregion

        #region Public Methods

        public static void CacheObject(IOCacheObject cache)
        {
            Task<bool> task = CacheObjectAsync(cache);
            task.Wait();
        }

        public async static Task<bool> CacheObjectAsync(IOCacheObject cache)
        {
            await InitializeCache();
            int exists = await CacheExists(cache);
            Task<bool> cacheTask;
            
            if (exists < 0)
            {
                cache.SetCacheTime();
                await InvalidateCacheAsync(cache.GetKey());

                cacheTask = factory.StartNew(() => {
                    CachedObjects.Add(cache);
                    return true;
                });
            }
            else 
            {
                cacheTask = factory.StartNew(() => {
                    return false;
                });
            }

            return await cacheTask;
        }

        public static void ClearCache() 
        {
            Task<bool> task1 = ClearCacheAsync();
            task1.Wait();

            Task<bool> task2 = InitializeCache();
            task2.Wait();
        }

        public async static Task<bool> ClearCacheAsync() 
        {
            Task<bool> cacheTask = factory.StartNew(() => {
                CachedObjects = null;
                return true;
            });

            return await cacheTask;
        }

        public static IOCacheObject GetCachedObject(string key)
        {
            Task<IOCacheObject> task = GetCachedObjectAsync(key);
            task.Wait();

            return task.Result;
        }

        public async static Task<IOCacheObject> GetCachedObjectAsync(string key)
        {
            await InitializeCache();

            Task<IOCacheObject> cacheTask = factory.StartNew(() => {
                IOCacheObject returnValue = null;
                if (CachedObjects != null && CachedObjects.Count() > 0)
                {
                    returnValue = CachedObjects.Find(obj => obj != null && obj.GetKey().Equals(key));
                }

                return returnValue;
            });

            return await cacheTask;
        }

        public static void InvalidateCache(string key)
        {
            Task<bool> task = InvalidateCacheAsync(key);
            task.Wait();
        }

        public async static Task<bool> InvalidateCacheAsync(string key)
        {
            await InitializeCache();

            Task<bool> cacheTask = factory.StartNew(() => {
                int index = CachedObjects.FindIndex(obj => obj != null && obj.GetKey().Equals(key));
                if (index >= 0 && index < CachedObjects.Count)
                {
                    try {
                        CachedObjects.RemoveAt(index);
                        return true;
                    } catch {
                    }
                }

                return false;
            });

            return await cacheTask;
        }

        #endregion

        #region Helper Methods

        private async static Task<int> CacheExists(IOCacheObject cache)
        {
			await InitializeCache();
            Task<int> cacheTask = factory.StartNew(() => {
                return CacheIndex(cache);
            });
            
            return await cacheTask;
        }

        private async static Task<bool> InitializeCache()
        {
            Task<bool> cacheTask = factory.StartNew(() => {
                if (CachedObjects == null)
                {
                    CachedObjects = new List<IOCacheObject>();
                }
                
                DateTimeOffset currentTimeOffset = DateTimeOffset.Now;
                long currentTimeStamp = currentTimeOffset.ToUnixTimeSeconds();
                List<int> forRemoveIndexes = new List<int>();

                foreach (IOCacheObject cache in IOCache.CachedObjects.ToList())
                {
                    if (cache == null)
                    {
                        CachedObjects.Remove(cache);
                    }
                    else if (cache.GetCacheEndTimeStamp() > 0 && cache.GetCacheEndTimeStamp() < currentTimeStamp)
                    {
                        int index = CacheIndex(cache);
                        if (index >= 0)
                        {
                            forRemoveIndexes.Add(index);
                        }
                    }
                }

                foreach (int index in forRemoveIndexes)
                {
                    if (index < CachedObjects.Count)
                    {
                        CachedObjects.RemoveAt(index);
                    }
                }

                return true;
            });

            return await cacheTask;
        }

        private static int CacheIndex(IOCacheObject cache)
        {
            return CachedObjects.FindIndex(obj => obj != null && obj.GetCacheID().Equals(cache.GetCacheID()));
        }

        #endregion
    }
}
