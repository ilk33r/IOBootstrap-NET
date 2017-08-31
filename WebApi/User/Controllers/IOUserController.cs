using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Entities.Users;
using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.WebApi.BackOffice.Controllers;
using IOBootstrap.NET.WebApi.User.Models;
using IOBootstrap.NET.WebApi.User.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Realms;
using System;
using System.Collections.Generic;

namespace IOBootstrap.NET.WebApi.User.Controllers
{
    public abstract class IOUserController<TLogger, TViewModel> : IOBackOfficeController<TLogger, TViewModel>
        where TViewModel : IOUserViewModel, new()
    {

        #region Controller Lifecycle

        public IOUserController(ILoggerFactory factory, ILogger<TLogger> logger, IConfiguration configuration, IIODatabase database)
            : base(factory, logger, configuration, database)
        {
        }

        #endregion

        #region User Methods

        [HttpPost]
        public IOAddUserResponseModel AddUser([FromBody] IOAddUserRequestModel requestModel) 
        {
			// Validate request
			if (requestModel == null
				|| requestModel.ClientInfo == null
                || String.IsNullOrEmpty(requestModel.UserName)
                || String.IsNullOrEmpty(requestModel.Password)
                || requestModel.Password.Length < 4)
			{
				// Obtain 400 error 
				IOResponseModel error400 = this.Error400("Invalid request data.");

				// Then return validation error
                return new IOAddUserResponseModel(new IOResponseStatusModel(error400.Status.Code, error400.Status.DetailedMessage), 0, null);
			}

			// Check client 
            if (!_viewModel.CheckClient(requestModel.ClientInfo))
			{
				// Then return invalid clients
				this.Response.StatusCode = 400;
                return new IOAddUserResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.INVALID_CLIENTS), 0, null);
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
			return new IOAddUserResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.BAD_REQUEST, "User exists"), 0, null);
		}

		[HttpPost]
        public IOResponseModel ChangePassword([FromBody] IOUserChangePasswordRequestModel requestModel) 
        {
			// Validate request
			if (requestModel == null
				|| requestModel.ClientInfo == null
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

			// Check client 
			if (!_viewModel.CheckClient(requestModel.ClientInfo))
			{
				// Then return invalid clients
				this.Response.StatusCode = 400;
				return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.INVALID_CLIENTS));
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
			if (requestModel == null
				|| requestModel.ClientInfo == null)
			{
				// Obtain 400 error 
				IOResponseModel error400 = this.Error400("Invalid request data.");

                // Then return validation error
                return error400;
			}

			// Check client 
            if (!_viewModel.CheckClient(requestModel.ClientInfo))
			{
				// Then return invalid clients
				this.Response.StatusCode = 400;
				return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.INVALID_CLIENTS));
			}

			// Obtain realm instance 
			Realm realm = _database.GetRealmForMainThread();

			// Obtain user entity
            IOUserEntity userEntity = realm.Find<IOUserEntity>(requestModel.UserId);

			// Check user entity is not null
			if (userEntity != null)
			{
				// Begin write transaction
				Transaction realmTransaction = realm.BeginWrite();

				// Delete all entity
				realm.Remove(userEntity);

				// Write transaction
				realmTransaction.Commit();

				// Then return response
				return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK));
			}

			// Dispose realm
			realm.Dispose();

			// Return bad request
			this.Response.StatusCode = 400;
			return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.BAD_REQUEST, "User not found."));
        }

        [HttpPost]
        public IOListUserResponseModel ListUsers([FromBody] IOListUserRequestModel requestModel) 
        {
			// Validate request
			if (requestModel == null
				|| requestModel.ClientInfo == null)
			{
				// Obtain 400 error 
				IOResponseModel error400 = this.Error400("Invalid request data.");

				// Then return validation error
                return new IOListUserResponseModel(new IOResponseStatusModel(error400.Status.Code, error400.Status.DetailedMessage), null);
			}

			// Check client 
            if (!_viewModel.CheckClient(requestModel.ClientInfo))
			{
				// Then return invalid clients
				this.Response.StatusCode = 400;
                return new IOListUserResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.INVALID_CLIENTS), null);
			}

            // Obtain user list
            List<IOUserInfoModel> users = _viewModel.ListUsers();

			// Create and return response
			return new IOListUserResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), users);
        }

        #endregion

    }
}
