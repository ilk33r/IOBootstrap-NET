using System;
using System.Collections.Generic;
using System.Linq;
using IOBootstrap.NET.Common.Entities.Configuration;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.WebApi.BackOffice.Models;

namespace IOBootstrap.NET.WebApi.BackOffice.ViewModels
{
    public class IOBackOfficeConfigurationsViewModel<TDBContext> : IOBackOfficeViewModel<TDBContext>
        where TDBContext : IODatabaseContext<TDBContext>
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
            _databaseContext.Add(configurationEntity);
            _databaseContext.SaveChanges();
        }

        public void DeleteConfigItem(int configurationId)
        {
            IOConfigurationEntity configuration = _databaseContext.Configurations.Find(configurationId);
            if (configuration != null)
            {
                _databaseContext.Remove(configuration);
                _databaseContext.SaveChanges();
            }
        }

        public IList<IOConfigurationEntity> GetConfigurations()
        {
            var configurations = _databaseContext.Configurations.OrderBy((arg) => arg.ID);
            return configurations.ToList();
        }

        public void UpdateConfigItem(IOConfigurationUpdateRequestModel requestModel)
        {
            // Obtain configuration item entity
            IOConfigurationEntity configurationEntity = _databaseContext.Configurations.Find(requestModel.ConfigId);

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
            _databaseContext.Update(configurationEntity);
            _databaseContext.SaveChanges();
        }

        #endregion
    }
}
