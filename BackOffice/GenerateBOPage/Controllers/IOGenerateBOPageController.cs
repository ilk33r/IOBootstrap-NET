using IOBootstrap.NET.BackOffice.Images.ViewModels;
using IOBootstrap.NET.Common;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.BackOffice;

[IOBackoffice]
public class IOGenerateBOPageController<TViewModel, TDBContext> : IOBackOfficeController<TViewModel, TDBContext>
where TDBContext : IODatabaseContext<TDBContext>
where TViewModel : IOGenerateBOPageViewModel<TDBContext>, new()
{
    #region Controller Lifecycle

    public IOGenerateBOPageController(IConfiguration configuration, IWebHostEnvironment environment, ILogger<IOLoggerType> logger, TDBContext databaseContext) : base(configuration, environment, logger, databaseContext)
    {
    }

    #endregion

    #region Controller Methods

    [IOValidateRequestModel]
    [IOUserRole(UserRoles.SuperAdmin)]
    [HttpPost("[action]")]
    public IOGenerateBOPageResponseModel CreateModel([FromBody] IOGenerateBOPageRequestModel requestModel)
    {
        // Create Model
        return ViewModel.CreateModel(requestModel.EntityName);
    }

    #endregion
}
