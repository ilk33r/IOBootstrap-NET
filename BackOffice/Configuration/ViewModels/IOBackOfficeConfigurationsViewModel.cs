using System;
using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Messages.Configuration;
using IOBootstrap.NET.Common.Models.Configuration;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.BackOffice.Configuration.Interfaces;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;
using IOBootstrap.NET.Common.Exceptions.Common;

namespace IOBootstrap.NET.BackOffice.Configuration.ViewModels;

public class IOBackOfficeConfigurationsViewModel<TDBContext> : IOBackOfficeViewModel<TDBContext>, IIOBackOfficeConfigurationsViewModel<TDBContext>
where TDBContext : IOBaseDatabaseContext<TDBContext>
{

    #region Initialization Methods

    public IOBackOfficeConfigurationsViewModel() : base()
    {
    }

    #endregion

    #region Menu Methods

    public void CheckConfigurationsIsEnabled()
    {
        bool isEnabled = Configuration.GetValue<bool>(IOConfigurationConstants.ConfigurationsEnabled);
        if (!isEnabled)
        {
            throw new IOInvalidAPIException();
        }
    }

    public virtual void AddConfigItem(IOConfigurationAddRequestModel requestModel)
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

        // Invalidate cache
        string cacheKey = IOCacheKeys.ConfigurationCacheKey + requestModel.ConfigKey;
        IOCache.InvalidateCache(cacheKey);
    }

    public virtual void DeleteConfigItem(int? configurationId)
    {
        IOConfigurationEntity? configuration = DatabaseContext.Configurations.Find(configurationId);
        if (configuration != null)
        {
            DatabaseContext.Remove(configuration);
            DatabaseContext.SaveChanges();

            IOConfigurationModel response = new IOConfigurationModel()
            {
                ID = configuration.ID,
                ConfigKey = configuration.ConfigKey
            };

            string cacheKey = IOCacheKeys.ConfigurationCacheKey + configuration.ConfigKey;
            IOCache.InvalidateCache(cacheKey);
        }
    }

    public virtual IList<IOConfigurationModel> GetConfigurations()
    {
        IList<IOConfigurationModel> configurations = DatabaseContext.Configurations
                                                                        .Select(c => new IOConfigurationModel()
                                                                        {
                                                                            ID = c.ID,
                                                                            ConfigKey = c.ConfigKey,
                                                                            ConfigIntValue = c.ConfigIntValue,
                                                                            ConfigStringValue = c.ConfigStringValue
                                                                        })
                                                                        .OrderBy(c => c.ID)
                                                                        .ToList();

        if (configurations == null)
        {
            return new List<IOConfigurationModel>();
        }

        return configurations;
    }

    public virtual void UpdateConfigItem(IOConfigurationUpdateRequestModel requestModel)
    {
        // Obtain configuration item entity
        IOConfigurationEntity? configurationEntity = DatabaseContext.Configurations.Find(requestModel.ConfigId);

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

        // Invalidate cache
        string cacheKey = IOCacheKeys.ConfigurationCacheKey + requestModel.ConfigKey;
        IOCache.InvalidateCache(cacheKey);
    }

    public virtual void RemoveLogs(DateTimeOffset? startDate)
    {
        // Obtain item entity
        IList<IOLogsEntity> logs = DatabaseContext.Logs
                                                        .Where(p => p.RequestDate < startDate)
                                                        .OrderBy(p => p.RequestDate)
                                                        .ToList();

        foreach (IOLogsEntity log in logs)
        {
            // Remove entity
            DatabaseContext.Remove(log);
        }

        DatabaseContext.SaveChanges();
    }

    public virtual void RemoveExceptions(DateTimeOffset? startDate)
    {
        // Obtain item entity
        IList<IOExceptionEntity> exceptions = DatabaseContext.Exceptions
                                                        .Where(p => p.RequestDate < startDate)
                                                        .OrderBy(p => p.RequestDate)
                                                        .ToList();

        foreach (IOExceptionEntity exception in exceptions)
        {
            // Remove entity
            DatabaseContext.Remove(exception);
        }

        DatabaseContext.SaveChanges();
    }

    #endregion
}
