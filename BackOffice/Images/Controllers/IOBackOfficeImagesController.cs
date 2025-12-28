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

namespace IOBootstrap.NET.BackOffice.Images.Controllers;

[IOBackoffice]
public class IOBackOfficeImagesController<TViewModel, TDBContext> : IOBackOfficeController<TViewModel, TDBContext>
where TDBContext : IOBaseDatabaseContext<TDBContext>
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

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 15)]
    [IOValidateRequestModel]
    [IOUserRole(UserRoles.BackOfficeUser)]
    [HttpPost("[action]")]
    public IOGetImagesResponseModel GetImages([FromBody] IOGetImagesRequestModel requestModel)
    {
        // Check enabled
        ViewModel.CheckImagesIsEnabled();

        return ViewModel.GetImages(requestModel);
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 15)]
    [IONonceRequired]
    [IOUserRole(UserRoles.BackOfficeUser)]
    [HttpPut("[action]")]
    public IOSaveImageResponseModel SaveImage(IFormFile file)
    {
        string filePath = ViewModel.SaveFile(file);
        IOImageVariationsModel imageMetadata = ViewModel.SaveImagesMetaData(filePath);
        return new IOSaveImageResponseModel(imageMetadata);
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 15)]
    [IOValidateRequestModel]
    [IONonceRequired]
    [IOUserRole(UserRoles.BackOfficeUser)]
    [HttpDelete("[action]")]
    public IOResponseModel DeleteImage([FromBody] IODeleteImagesRequestModel requestModel)
    {
        ViewModel.DeleteImage(requestModel);
        return new IOResponseModel();
    }

    #endregion
}
