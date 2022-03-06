using System;
using IOBootstrap.NET.Common.Messages.Configuration;
using IOBootstrap.NET.Common.Models.Configuration;
using IOBootstrap.NET.MW.Core.ViewModels;
using IOBootstrap.NET.MW.DataAccess.Context;
using IOBootstrap.NET.MW.DataAccess.Entities;

namespace IOBootstrap.NET.MW.WebApi.BackOffice.ViewModels
{
    public class IOBackOfficeConfigurationsViewModel<TDBContext> : IOMWViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {

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
        }

        public IOConfigurationModel DeleteConfigItem(int configurationId)
        {
            IOConfigurationEntity configuration = DatabaseContext.Configurations.Find(configurationId);
            if (configuration != null)
            {
                DatabaseContext.Remove(configuration);
                DatabaseContext.SaveChanges();

                IOConfigurationModel response = new IOConfigurationModel()
                {
                    ID = configuration.ID,
                    ConfigKey = configuration.ConfigKey
                };

                return response;
            }

            return null;
        }
        
        public IList<IOConfigurationModel> ListConfigurationItems()
        {
            return DatabaseContext.Configurations
                                            .Select(c => new IOConfigurationModel()
                                            {
                                                ID = c.ID,
                                                ConfigKey = c.ConfigKey,
                                                ConfigIntValue = c.ConfigIntValue,
                                                ConfigStringValue = c.ConfigStringValue
                                            })
                                            .ToList();
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
        }

        public IOConfigurationModel GetConfigItem(string key)
        {
            return DatabaseContext.Configurations
                                            .Select(c => new IOConfigurationModel()
                                            {
                                                ConfigKey = c.ConfigKey,
                                                ConfigIntValue = c.ConfigIntValue,
                                                ConfigStringValue = c.ConfigStringValue
                                            })
                                            .FirstOrDefault();
        }
    }
}
