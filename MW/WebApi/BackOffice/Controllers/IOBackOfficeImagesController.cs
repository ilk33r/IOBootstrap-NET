using System;
using IOBootstrap.NET.Common.Messages.MW;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Images;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.MW.Core.Controllers;
using IOBootstrap.NET.MW.DataAccess.Context;
using IOBootstrap.NET.MW.WebApi.BackOffice.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.MW.WebApi.BackOffice.Controllers
{
    public class IOBackOfficeImagesController<TViewModel, TDBContext> : IOMWController<TViewModel, TDBContext> where TViewModel : IOBackOfficeImagesViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {
        public IOBackOfficeImagesController(IConfiguration configuration, 
                                            TDBContext databaseContext, 
                                            IWebHostEnvironment environment, 
                                            ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOMWListResponseModel<IOImageVariationsModel> GetImages([FromBody] IOGetImagesRequestModel requestModel)
        {
            return ViewModel.GetImages(requestModel.Start, requestModel.Count);
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOMWListResponseModel<IOImageVariationsModel> SaveImages([FromBody] IOMWSaveImageRequestModel requestModel)
        {
            IList<IOImageVariationsModel> imageVariations = ViewModel.SaveImages(requestModel);
            return new IOMWListResponseModel<IOImageVariationsModel>(imageVariations);
        }

        [IORequireHTTPS]
        [HttpPost]
        public IOMWListResponseModel<IOImageVariationsModel> DeleteImages([FromBody] IODeleteImagesRequestModel requestModel)
        {
            IList<IOImageVariationsModel> imageVariations = ViewModel.DeleteImages(requestModel);
            return new IOMWListResponseModel<IOImageVariationsModel>(imageVariations);
        }
    }
}
