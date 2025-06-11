using System;
using System.Threading.Tasks;
using IOBootstrap.NET.BackOffice.User.Interfaces;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Authentication;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.Users;
using IOBootstrap.NET.Common.Models.Users;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.BackOffice.User.Controllers;

[IOBackoffice]
public abstract class IOUserController<TViewModel, TDBContext> : IOBackOfficeController<TViewModel, TDBContext>
where TDBContext : IODatabaseContext<TDBContext>
where TViewModel : IIOUserViewModel<TDBContext>, new()
{
    #region Controller Lifecycle

    protected IOUserController(IConfiguration configuration,
                               IWebHostEnvironment environment,
                               ILogger<IOLoggerType> logger,
                               TDBContext databaseContext) : base(configuration, environment, logger, databaseContext)
    {
    }

    #endregion

    #region User Methods

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 15)]
    [IOValidateRequestModel]
    [IOEncryptionRequired]
    [IOUserRole(UserRoles.Admin)]
    [HttpPost("[action]")]
    public virtual async Task<IOAddUserResponseModel> AddUser([FromBody] IOAddUserRequestModel requestModel)
    {
        // Create and return response
        return await ViewModel.AddUser(requestModel);
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 1)]
    [IOValidateRequestModel]
    [IOEncryptionRequired]
    [IOIgnorePasswordExpire]
    [IOUserRole(UserRoles.BackOfficeUser)]
    [HttpPost("[action]")]
    public virtual async Task<IOResponseModel> ChangePassword([FromBody] IOUserChangePasswordRequestModel requestModel)
    {
        // Check change password is success
        await ViewModel.ChangePassword(requestModel.OldPassword ?? String.Empty, requestModel.NewPassword ?? String.Empty);

        // Return response
        return new IOResponseModel();
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 2)]
    [IOValidateRequestModel]
    [IOEncryptionRequired]
    [IOUserRole(UserRoles.Admin)]
    [HttpPost("[action]")]
    public virtual async Task<IOResponseModel> ResetPassword([FromBody] IOUserResetPasswordRequestModel requestModel)
    {
        // Reset user password
        await ViewModel.ResetPassword(requestModel.UserName ?? String.Empty, requestModel.NewPassword ?? String.Empty);

        // Return response
        return new IOResponseModel();
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 15)]
    [IOUserRole(UserRoles.Admin)]
    [HttpGet("[action]")]
    public virtual IOListUserResponseModel ListUsers()
    {
        // Obtain user list
        IList<IOUserInfoModel> users = ViewModel.ListUsers();

        // Create and return response
        return new IOListUserResponseModel(users);
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 5)]
    [IOValidateRequestModel]
    [IOUserRole(UserRoles.Admin)]
    [HttpPost("[action]")]
    public virtual IOUpdateUserResponseModel UpdateUser([FromBody] IOUpdateUserRequestModel requestModel)
    {
        ViewModel.UpdateUser(requestModel);
        return new IOUpdateUserResponseModel();
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 3)]
    [IOValidateRequestModel]
    [IOUserRole(UserRoles.Admin)]
    [HttpPost("[action]")]
    public virtual IOResponseModel DeleteUser([FromBody] IODeleteUserRequestModel requestModel)
    {
        ViewModel.DeleteUser(requestModel);
        return new IOResponseModel();
    }

    [IORequireHTTPS]
    [IORateLimit(seconds: 60, requestCount: 15)]
    [IOValidateRequestModel]
    [IOEncryptionRequired]
    [IOUserRole(UserRoles.BackOfficeUser)]
    [IOIgnorePasswordExpire]
    [HttpPost("[action]")]
    public virtual IOResponseModel Logout([FromBody] IOLogoutRequestModel requestModel)
    {
        // Check if authentication result is true
        ViewModel.Logout(requestModel.UserName ?? "");
        return new IOResponseModel();
    }
    
    #endregion

}
