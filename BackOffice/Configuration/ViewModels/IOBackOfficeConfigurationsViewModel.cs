using System;
using System.Collections.Generic;
using System.Linq;
using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Messages.Configuration;
using IOBootstrap.NET.Common.Models.Configuration;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.BackOffice.Configuration.ViewModels
{
    public class IOBackOfficeConfigurationsViewModel<TDBContext> : IOBackOfficeViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Initialization Methods

        public IOBackOfficeConfigurationsViewModel() : base()
        {
        }

        #endregion

        #region Menu Methods

        public void AddConfigItem(IOConfigurationAddRequestModel requestModel)
        {
            // Create configuration item entity
            IOConfigurationEntity configurationEntity = new IOConfigurationEntity()
            {
                ConfigKey = requestModel.ConfigKey,
                ConfigIntValue = requestModel.IntValue,
                ConfigStringValue = requestModel.StrValue
            };

            // Add config entity to database
            DatabaseContext.Add(configurationEntity);
            DatabaseContext.SaveChanges();

            string cacheKey = IOCacheKeys.ConfigurationCacheKey + requestModel.ConfigKey;
            IOCache.InvalidateCache(cacheKey);
        }

        public void DeleteConfigItem(int configurationId)
        {
            IOConfigurationEntity configuration = DatabaseContext.Configurations.Find(configurationId);
            if (configuration != null)
            {
                DatabaseContext.Remove(configuration);
                DatabaseContext.SaveChanges();

                string cacheKey = IOCacheKeys.ConfigurationCacheKey + configuration.ConfigKey;
                IOCache.InvalidateCache(cacheKey);
            }
        }

        public IList<IOConfigurationModel> GetConfigurations()
        {
            var configurations = DatabaseContext.Configurations.OrderBy((arg) => arg.ID);
            return configurations.ToList()
                                .ConvertAll(configuration => {
                                    IOConfigurationModel configurationModel = new IOConfigurationModel();
                                    configurationModel.ID = configuration.ID;
                                    configurationModel.ConfigKey = configuration.ConfigKey;
                                    configurationModel.ConfigIntValue = configuration.ConfigIntValue;
                                    configurationModel.ConfigStringValue = configuration.ConfigStringValue;

                                    return configurationModel;
                                });
        }

        public void UpdateConfigItem(IOConfigurationUpdateRequestModel requestModel)
        {
            // Obtain configuration item entity
            IOConfigurationEntity configurationEntity = DatabaseContext.Configurations.Find(requestModel.ConfigId);

            // Check config is not exists
            if (configurationEntity == null)
            {
                return;
            }

            // Update config item entity
            configurationEntity.ConfigKey = requestModel.ConfigKey;
            configurationEntity.ConfigIntValue = requestModel.IntValue;
            configurationEntity.ConfigStringValue = requestModel.StrValue;

            // Add menu entity to database
            DatabaseContext.Update(configurationEntity);
            DatabaseContext.SaveChanges();

            string cacheKey = IOCacheKeys.ConfigurationCacheKey + configurationEntity.ConfigKey;
            IOCache.InvalidateCache(cacheKey);
        }

        #endregion
    }
}
