using System;
using System.Collections.Generic;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Entities.Users;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.WebApi.User.Models;
using IOBootstrap.NET.WebApi.User.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.WebApi.User.Controllers
{
    public abstract class IOUserController<TLogger, TViewModel, TDBContext> : IOBackOfficeController<TLogger, TViewModel, TDBContext>
        where TViewModel : IOUserViewModel<TDBContext>, new()
        where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Controller Lifecycle

        public IOUserController(ILoggerFactory factory, 
                                ILogger<TLogger> logger, 
                                IConfiguration configuration, 
                                TDBContext databaseContext,
                                IHostingEnvironment environment)
            : base(factory, logger, configuration, databaseContext, environment)
        {
        }

        #endregion

        #region User Methods

        [HttpPost]
        public virtual IOAddUserResponseModel AddUser([FromBody] IOAddUserRequestModel requestModel) 
        {
			// Validate request
			if (requestModel == null
                || String.IsNullOrEmpty(requestModel.UserName)
                || String.IsNullOrEmpty(requestModel.Password)
                || requestModel.Password.Length < 4)
			{
				// Obtain 400 error 
				IOResponseModel error400 = this.Error400("Invalid request data.");

				// Then return validation error
                return new IOAddUserResponseModel(new IOResponseStatusModel(error400.Status.Code, error400.Status.DetailedMessage), 0, null);
			}

            // Check current user role
            if (!UserRoleUtility.CheckRole(UserRoles.Admin, (UserRoles)_viewModel.userEntity.UserRole)) 
            {
                // Update response status code
                this.Response.StatusCode = 401;

                // Create response status model
                IOResponseStatusModel responseStatus = new IOResponseStatusModel(IOResponseStatusMessages.INVALID_PERMISSION, "Permission denied.");

                // Return response
                return new IOAddUserResponseModel(responseStatus, 0, null);
            }

            // Obtain add user response
            Tuple<bool, int, string> addUserStatus = _viewModel.AddUser(requestModel.UserName, requestModel.Password, requestModel.UserRole);

            // Check add user is success
            if (addUserStatus.Item1) 
            {
				// Create and return response
                return new IOAddUserResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), addUserStatus.Item2, addUserStatus.Item3);
            }

			// Return user exists response
			this.Response.StatusCode = 400;
            return new IOAddUserResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.USER_EXISTS, "User exists"), 0, null);
		}

		[HttpPost]
        public IOResponseModel ChangePassword([FromBody] IOUserChangePasswordRequestModel requestModel) 
        {
			// Validate request
			if (requestModel == null
                || String.IsNullOrEmpty(requestModel.UserName)
                || String.IsNullOrEmpty(requestModel.OldPassword)
                || String.IsNullOrEmpty(requestModel.NewPassword)
                || requestModel.NewPassword.Length < 4)
			{
				// Obtain 400 error 
				IOResponseModel error400 = this.Error400("Invalid request data.");

                // Then return validation error
                return error400;
			}

            // Check change password is success
            if (_viewModel.ChangePassword(requestModel.UserName, requestModel.OldPassword, requestModel.NewPassword))
            {
				// Return response
				return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK));
            }

            // Return response
			this.Response.StatusCode = 400;
            return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.BAD_REQUEST, "Invalid user."));
        }

        [HttpPost]
        public IOResponseModel DeleteUser([FromBody] IODeleteUserRequestModel requestModel) 
        {
			// Validate request
			if (requestModel == null)
			{
				// Obtain 400 error 
				IOResponseModel error400 = this.Error400("Invalid request data.");

                // Then return validation error
                return error400;
			}

            // Obtain user entity
            IOUserEntity userEntity = _databaseContext.Users.Find(requestModel.UserId);

			// Check user entity is not null
			if (userEntity != null)
			{
                // Delete all entity
                _databaseContext.Remove(userEntity);
                _databaseContext.SaveChanges();

				// Then return response
				return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK));
			}

			// Return bad request
			this.Response.StatusCode = 400;
			return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.BAD_REQUEST, "User not found."));
        }

        [HttpGet]
        public IOListUserResponseModel ListUsers() 
        {
            // Obtain user list
            List<IOUserInfoModel> users = _viewModel.ListUsers();

			// Create and return response
			return new IOListUserResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), users);
        }

        [HttpPost]
        public IOUpdateUserResponseModel UpdateUser([FromBody] IOUpdateUserRequestModel requestModel)
        {
            // Validate request
            if (requestModel == null)
            {
                // Obtain 400 error 
                IOResponseModel error400 = this.Error400("Invalid request data.");

                // Then return validation error
                return new IOUpdateUserResponseModel(error400.Status);
            }

            int status = _viewModel.UpdateUser(requestModel);
            IOResponseStatusModel responseStatus;

            if (status == 3)
            {
                // Update response status code
                this.Response.StatusCode = 400;

                // Obtain 400 error 
                responseStatus = new IOResponseStatusModel(IOResponseStatusMessages.USER_EXISTS, "User name exists.");
                return new IOUpdateUserResponseModel(responseStatus);
            }
            else if (status == 2)
            {
                // Update response status code
                this.Response.StatusCode = 400;

                // Obtain 400 error 
                responseStatus = new IOResponseStatusModel(IOResponseStatusMessages.INVALID_PERMISSION, "Permission denied.");
                return new IOUpdateUserResponseModel(responseStatus);
            }
            else if (status == 1)
            {
                // Update response status code
                this.Response.StatusCode = 400;

                // Obtain 400 error 
                responseStatus = new IOResponseStatusModel(IOResponseStatusMessages.USER_NOT_FOUND, "User not found.");
                return new IOUpdateUserResponseModel(responseStatus);
            }

            responseStatus = new IOResponseStatusModel(IOResponseStatusMessages.OK);
            return new IOUpdateUserResponseModel(responseStatus);
        }

        #endregion

    }
}
