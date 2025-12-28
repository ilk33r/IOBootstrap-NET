using System;
using IOBootstrap.NET.BackOffice.Logs.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Logs;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.BackOffice.Logs.Controllers;

[IOBackoffice]
public class IOBackOfficeLogsController<TViewModel, TDBContext> : IOBackOfficeController<TViewModel, TDBContext>
where TDBContext : IOBaseDatabaseContext<TDBContext>
where TViewModel : IOBackOfficeLogsViewModel<TDBContext>, new()
{
    #region Controller Lifecycle

    public IOBackOfficeLogsController(IConfiguration configuration,
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
    [IOUserRole(UserRoles.SuperAdmin)]
    [HttpPost("[action]")]
    public IOGetLogsResponseModel GetLogs([FromBody] IOGetLogsRequestModel requestModel)
    {
        return ViewModel.GetLogs(requestModel);
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 15)]
    [IOValidateRequestModel]
    [IOUserRole(UserRoles.SuperAdmin)]
    [HttpPost("[action]")]
    public IOGetLogsResponseModel GetExceptions([FromBody] IOGetLogsRequestModel requestModel)
    {
        return ViewModel.GetExceptions(requestModel);
    }

    #endregion
}
