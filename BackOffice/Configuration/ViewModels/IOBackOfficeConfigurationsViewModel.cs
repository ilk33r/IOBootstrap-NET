using System;
using IOBootstrap.NET.Common.Messages.MW;
using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.Configuration;
using IOBootstrap.NET.Common.Models.Configuration;
using IOBootstrap.NET.Core.ViewModels;

namespace IOBootstrap.NET.BackOffice.Configuration.ViewModels
{
    public class IOBackOfficeConfigurationsViewModel : IOBackOfficeViewModel
    {

        #region Initialization Methods

        public IOBackOfficeConfigurationsViewModel() : base()
        {
        }

        #endregion

        #region Menu Methods

        public void AddConfigItem(IOConfigurationAddRequestModel requestModel)
        {
            // MW connection
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeConfigurationControllerNameKey);
            IOResponseModel configurations = MWConnector.Get<IOMWListResponseModel<IOResponseModel>>(controller + "/" + "AddConfigItem", requestModel);
            if (MWConnector.HandleResponse(configurations, code => {}))
            {
                // Invalidate cache
                string cacheKey = IOCacheKeys.ConfigurationCacheKey + requestModel.ConfigKey;
                IOCache.InvalidateCache(cacheKey);
            }
        }

        public void DeleteConfigItem(int configurationId)
        {
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeConfigurationControllerNameKey);
            IOMWFindRequestModel requestModel = new IOMWFindRequestModel()
            {
                ID = configurationId
            };
            IOMWObjectResponseModel<IOConfigurationModel> responseModel = MWConnector.Get<IOMWObjectResponseModel<IOConfigurationModel>>(controller + "/" + "DeleteConfigItem", requestModel);
            if (MWConnector.HandleResponse(responseModel, code => {}))
            {
                string cacheKey = IOCacheKeys.ConfigurationCacheKey + responseModel.Item.ConfigKey;
                IOCache.InvalidateCache(cacheKey);
            }
        }

        public IList<IOConfigurationModel> GetConfigurations()
        {
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeConfigurationControllerNameKey);
            IOMWListResponseModel<IOConfigurationModel> configurations = MWConnector.Get<IOMWListResponseModel<IOConfigurationModel>>(controller + "/" + "ListConfigurationItems", new IOMWFindRequestModel());
            if (MWConnector.HandleResponse(configurations, code => {}))
            {
                return configurations.Items;
            }

            return new List<IOConfigurationModel>();
        }

        public void UpdateConfigItem(IOConfigurationUpdateRequestModel requestModel)
        {
            // MW connection
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeConfigurationControllerNameKey);
            IOResponseModel configurations = MWConnector.Get<IOMWListResponseModel<IOResponseModel>>(controller + "/" + "UpdateConfigItem", requestModel);
            if (MWConnector.HandleResponse(configurations, code => {}))
            {
                // Invalidate cache
                string cacheKey = IOCacheKeys.ConfigurationCacheKey + requestModel.ConfigKey;
                IOCache.InvalidateCache(cacheKey);
            }
        }

        #endregion
    }
}
