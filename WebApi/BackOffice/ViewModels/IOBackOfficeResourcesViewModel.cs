using System;
using IOBootstrap.NET.Common.Entities.Resource;
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
    }
}
