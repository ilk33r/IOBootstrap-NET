using System;
using System.Collections.Generic;
using System.Linq;
using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Messages.Resources;
using IOBootstrap.NET.Common.Models.Resources;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.BackOffice.Resources.ViewModels
{
    public class IOBackOfficeResourcesViewModel<TDBContext> : IOBackOfficeViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
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
            DatabaseContext.Add(resourceEntity);

            try {
                DatabaseContext.SaveChanges();
            } catch {
            }

            string cacheKey = IOCacheKeys.ResourceCacheKey + requestModel.ResourceKey;
            IOCache.InvalidateCache(cacheKey);
        }

        public void DeleteResourceItem(int resourceId)
        {
            IOResourceEntity resource = DatabaseContext.Resources.Find(resourceId);
            if (resource == null) {
                return;
            }

            string cacheKey = IOCacheKeys.ResourceCacheKey + resource.ResourceKey;

            DatabaseContext.Remove(resource);
            DatabaseContext.SaveChanges();

            IOCache.InvalidateCache(cacheKey);
        }

        public IList<IOResourceModel> GetAllResources()
        {
            List<IOResourceModel> resouces = new List<IOResourceModel>();
            var resourcesEntity = DatabaseContext.Resources.OrderBy((arg) => arg.ID);

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
                IOResourceEntity resource = IOResourceEntity.ResourceForKey(resourceKey, DatabaseContext);
                IOResourceModel resourceModel = new IOResourceModel();
                resourceModel.ResourceID = 0;
                resourceModel.ResourceKey = resourceKey;
                resourceModel.ResourceValue = (resource != null) ? resource.ResourceValue : resourceKey;
                resouces.Add(resourceModel);
            }

            return resouces;
        }

        public void UpdateResourceItem(IOResourceUpdateRequestModel requestModel) {
            IOResourceEntity resource = DatabaseContext.Resources.Find(requestModel.ResourceID);
            if (resource == null) {
                return;
            }

            resource.ResourceKey = requestModel.ResourceKey;
            resource.ResourceValue = requestModel.ResourceValue;
            DatabaseContext.Update(resource);
            DatabaseContext.SaveChanges();

            string cacheKey = IOCacheKeys.ResourceCacheKey + requestModel.ResourceKey;
            IOCache.InvalidateCache(cacheKey);
        } 
    }
}
