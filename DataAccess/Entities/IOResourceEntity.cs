using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.DataAccess.Entities
{
    public class IOResourceEntity
    {
        #region Properties

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [StringLength(128)]
        public string ResourceKey { get; set; }

        public string ResourceValue { get; set; }

        #endregion

        #region Helper Methods

        public static IOResourceEntity ResourceForKey<TDBContext>(string resourceKey, TDBContext inContext) where TDBContext : IODatabaseContext<TDBContext>
        {
            string cacheKey = IOCacheKeys.ResourceCacheKey + resourceKey;
            IOCacheObject cachedObject = IOCache.GetCachedObject(cacheKey);
            if (cachedObject != null)
            {
                IOResourceEntity resourceEntity = (IOResourceEntity)cachedObject.Value;
                return resourceEntity;
            }
            
            var resources = inContext.Resources.Where((arg) => arg.ResourceKey.Equals(resourceKey));

            if (resources.Count() > 0)
            {
                IOResourceEntity resource = resources.First();
                cachedObject = new IOCacheObject(cacheKey, resource, 36000);
                IOCache.CacheObject(cachedObject);
                return resource;
            }
            
            return null;
        }

        #endregion
    }
}
