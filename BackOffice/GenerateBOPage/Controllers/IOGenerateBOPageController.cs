using IOBootstrap.NET.BackOffice.GenerateBOPage.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.GenerateBOPage;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;

#if DEBUG
namespace IOBootstrap.NET.BackOffice.GenerateBOPage.Controllers;

[IOBackoffice]
public class IOGenerateBOPageController<TViewModel, TDBContext> : IOBackOfficeController<TViewModel, TDBContext>
where TDBContext : IOBaseDatabaseContext<TDBContext>
where TViewModel : IOGenerateBOPageViewModel<TDBContext>, new()
{
    #region Controller Lifecycle

    public IOGenerateBOPageController(IConfiguration configuration, IWebHostEnvironment environment, ILogger<IOLoggerType> logger, TDBContext databaseContext) : base(configuration, environment, logger, databaseContext)
    {
    }

    #endregion

    #region Controller Methods

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 15)]
    [IOValidateRequestModel]
    [IOUserRole(UserRoles.SuperAdmin)]
    [HttpPost("[action]")]
    public IOGenerateBOPageResponseModel CreateModel([FromBody] IOGenerateBOPageRequestModel requestModel)
    {
        // Create Model
        return ViewModel.CreateModel(requestModel.EntityName ?? "");
    }

    #endregion
}
#endif