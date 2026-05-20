using System;
using IOBootstrap.NET.BackOffice.Files.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.Files;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.BackOffice.Files.Controllers;

[IOBackoffice]
public class IOBackOfficeFilesController<TViewModel, TDBContext> : IOBackOfficeController<TViewModel, TDBContext>
where TDBContext : IOBaseDatabaseContext<TDBContext>
where TViewModel : IOBackOfficeFilesViewModel<TDBContext>, new()
{
    #region Controller Lifecycle

    public IOBackOfficeFilesController(IConfiguration configuration,
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
    public IOGetFilesResponseModel GetFiles([FromBody] IOGetFilesRequestModel requestModel)
    {
        ViewModel.CheckFilesIsEnabled();
        return ViewModel.GetFiles(requestModel);
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 15)]
    [IONonceRequired]
    [IOUserRole(UserRoles.BackOfficeUser)]
    [HttpPut("[action]")]
    public IOSaveFileResponseModel SaveFile(IFormFile file, [FromForm] string description)
    {
        string filePath = ViewModel.SaveFile(file);
        IOFileVariationsModel fileMetadata = ViewModel.SaveFilesMetaData(filePath, file.ContentType, description);
        return new IOSaveFileResponseModel(fileMetadata);
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 15)]
    [IOValidateRequestModel]
    [IONonceRequired]
    [IOUserRole(UserRoles.BackOfficeUser)]
    [HttpDelete("[action]")]
    public IOResponseModel DeleteFile([FromBody] IODeleteFilesRequestModel requestModel)
    {
        ViewModel.DeleteFile(requestModel);
        return new IOResponseModel();
    }

    #endregion
}
