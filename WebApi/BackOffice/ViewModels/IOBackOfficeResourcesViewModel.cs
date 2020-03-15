using System;
using System.Collections.Generic;
using System.Linq;
using IOBootstrap.NET.Common.Entities.Resource;
using IOBootstrap.NET.Core.Cache;
using IOBootstrap.NET.Core.Cache.Utilities;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.WebApi.BackOffice.Models;

namespace IOBootstrap.NET.WebApi.BackOffice.ViewModels
{
    public class IOBackOfficeResourcesViewModel<TDBContext> : IOBackOfficeViewModel<TDBContext>
        where TDBContext : IODatabaseContext<TDBContext>
    {
        #region Initialization Methods

        public IOBackOfficeResourcesViewModel() : base()
        {
        }

        #endregion

        public void AddResourceItem(IOResourceAddRequestModel requestModel)
        {
            // Create resource item entity
            IOResourceEntity resourceEntity = new IOResourceEntity()
            {
                ResourceKey = requestModel.ResourceKey,
                ResourceValue = requestModel.ResourceValue
            };

            // Add resource entity to database
            _databaseContext.Add(resourceEntity);
            _databaseContext.SaveChanges();

            string cacheKey = "IOResourceCache" + requestModel.ResourceKey;
            IOCache.InvalidateCache(cacheKey);
        }

        public IList<IOResourceModel> GetAllResources()
        {
            List<IOResourceModel> resouces = new List<IOResourceModel>();
            var resourcesEntity = _databaseContext.Resources.OrderBy((arg) => arg.ID);

            foreach (IOResourceEntity resource in resourcesEntity)
            {
                IOResourceModel resourceModel = new IOResourceModel();
                resourceModel.ResourceID = resource.ID;
                resourceModel.ResourceKey = resource.ResourceKey;
                resourceModel.ResourceValue = resource.ResourceValue;
                resouces.Add(resourceModel);
            }

            return resouces;
        }

        public IList<IOResourceModel> GetResources(IList<string> resourceKeys)
        {
            List<IOResourceModel> resouces = new List<IOResourceModel>();

            foreach (string resourceKey in resourceKeys)
            {
                IOResourceEntity resource = IOResourceEntity.ResourceForKey(resourceKey, _databaseContext);
                IOResourceModel resourceModel = new IOResourceModel();
                resourceModel.ResourceID = 0;
                resourceModel.ResourceKey = resourceKey;
                resourceModel.ResourceValue = (resource != null) ? resource.ResourceValue : resourceKey;
                resouces.Add(resourceModel);
            }

            return resouces;
        }

        public void UpdateResourceItem(IOResourceUpdateRequestModel requestModel) {
            IOResourceEntity resource = _databaseContext.Resources.Find(requestModel.ResourceID);
            if (resource == null) {
                return;
            }

            resource.ResourceKey = requestModel.ResourceKey;
            resource.ResourceValue = requestModel.ResourceValue;
            _databaseContext.Update(resource);
            _databaseContext.SaveChanges();

            string cacheKey = "IOResourceCache" + requestModel.ResourceKey;
            IOCache.InvalidateCache(cacheKey);
        } 
    }
}
