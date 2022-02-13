using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;
using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Models.Base;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.DataAccess.Entities
{
    public class IOConfigurationEntity
    {

        #region Properties

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [StringLength(128)]
        public string ConfigKey { get; set; }

        public int? ConfigIntValue { get; set; }
        public string ConfigStringValue { get; set; }

        #endregion

        #region Helper Methods

        public static IOConfigurationEntity ConfigForKey<TDBContext>(string configKey, TDBContext inContext) where TDBContext : IODatabaseContext<TDBContext>
        {
            string cacheKey = IOCacheKeys.ConfigurationCacheKey + configKey;
            IOCacheObject cachedObject = IOCache.GetCachedObject(cacheKey);
            if (cachedObject != null)
            {
                IOConfigurationEntity configurationEntity = (IOConfigurationEntity)cachedObject.Value;
                return configurationEntity;
            }

            var configurations = inContext.Configurations.Where((arg) => arg.ConfigKey.Equals(configKey));

            if (configurations.Count() > 0)
            {
                IOConfigurationEntity configuration = configurations.First();
                cachedObject = new IOCacheObject(cacheKey, configuration, 36000);
                IOCache.CacheObject(cachedObject);
                return configuration;
            }

            return null;
        }

        public int IntValue()
        {
            return this.ConfigIntValue ?? 0;
        }

        public string StringValue()
        {
            return this.ConfigStringValue ?? "";
        }

        public TModel ObjectValue<TModel>() where TModel : IOModel, new()
        {
            if (String.IsNullOrEmpty(this.ConfigStringValue))
            {
                return new TModel();
            }
                
            return JsonSerializer.Deserialize<TModel>(this.ConfigStringValue);
        }

        #endregion

    }
}
