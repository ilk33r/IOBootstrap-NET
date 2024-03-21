using System;
using IOBootstrap.NET.BackOffice.Images.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.Images;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.Core.Extensions;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.BackOffice.Images.Controllers
{
    [IOBackoffice]
    public class IOBackOfficeImagesController<TViewModel, TDBContext> : IOBackOfficeController<TViewModel, TDBContext> 
    where TDBContext : IODatabaseContext<TDBContext> 
    where TViewModel : IOBackOfficeImagesViewModel<TDBContext>, new()
    {
        #region Controller Lifecycle

        public IOBackOfficeImagesController(IConfiguration configuration, 
                                            IWebHostEnvironment environment, 
                                            ILogger<IOLoggerType> logger,
                                            TDBContext databaseContext) : base(configuration, environment, logger, databaseContext)
        {
        }

        #endregion

        #region API Methods

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.CustomUser)]
        [HttpPost("[action]")]
        public IOGetImagesResponseModel GetImages([FromBody] IOGetImagesRequestModel requestModel)
        {
            return ViewModel.GetImages(requestModel);
        }

        [IORequireHTTPS]
        [IOUserRole(UserRoles.CustomUser)]
        [HttpPut("[action]")]
        public IOSaveImageResponseModel SaveImage(IFormFile file)
        {
            string filePath = ViewModel.SaveFile(file);
            IOImageVariationsModel imageMetadata = ViewModel.SaveImagesMetaData(filePath, file.FileName);
            return new IOSaveImageResponseModel(imageMetadata);
        }

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.CustomUser)]
        [HttpPost("[action]")]
        public IOResponseModel DeleteImages([FromBody] IODeleteImagesRequestModel requestModel)
        {
            ViewModel.DeleteImages(requestModel);
            return new IOResponseModel();
        }

        #endregion
    }
}
