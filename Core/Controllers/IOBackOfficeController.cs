using System;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Exceptions.Members;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Core.Interfaces;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IOBootstrap.NET.Core.Controllers;

[IOBackoffice]
public abstract class IOBackOfficeController<TViewModel, TDBContext> : IOController<TViewModel, TDBContext>
where TDBContext : IODatabaseContext<TDBContext>
where TViewModel : IIOBackOfficeViewModel<TDBContext>, new()
{

    #region Controller Lifecycle

    public IOBackOfficeController(IConfiguration configuration,
                                  IWebHostEnvironment environment,
                                  ILogger<IOLoggerType> logger,
                                  TDBContext databaseContext) : base(configuration, environment, logger, databaseContext)
    {
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Update view model request value
        ViewModel.Request = Request;

        if (!ViewModel.IsBackOffice())
        {
            throw new IOInvalidPermissionException();
        }

        base.OnActionExecuting(context);

        // Check user password expired
        CheckUserPasswordExpired(context);
    }

    #endregion

    #region Security

    [IOIgnorePasswordExpire]
    [IOUserRole(UserRoles.BackOfficeUser)]
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet("[action]")]
    public IOResponseModel GenerateNonce()
    {
        return new IOResponseModel();
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public virtual void CheckUserPasswordExpired(ActionExecutingContext context)
    {
        // Check password expire is ignored
        if (HasControllerAttribute<IOIgnorePasswordExpireAttribute>(context))
        {
            // Do nothing
            return;
        }

        // Obtain password expired date
        DateTimeOffset? passwordExpiredDate = ViewModel.UserModel?.PasswordExpireDate;

        // Check password expire date is defined
        if (passwordExpiredDate == null)
        {
            // Do nothing
            return;
        }

        // Obtain values
        DateTimeOffset passwordExpiredDateOffset = (DateTimeOffset)passwordExpiredDate!;
        long currentUnixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long passwordExpireUnixTimeSeconds = passwordExpiredDateOffset.ToUnixTimeSeconds();

        // Check password is expired
        if (passwordExpireUnixTimeSeconds < currentUnixTimeSeconds)
        {
            // Then thrown an exception
            throw new IOPasswordExpiredException();
        }
    }

    #endregion
}
