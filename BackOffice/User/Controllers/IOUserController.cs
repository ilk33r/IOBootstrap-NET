using System;
using System.Collections.Generic;
using IOBootstrap.NET.BackOffice.User.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Exceptions.Members;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.Users;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Common.Models.Users;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.WebApi.User.Controllers
{
    public abstract class IOUserController<TViewModel, TDBContext> : IOBackOfficeController<TViewModel, TDBContext> where TViewModel : IOUserViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {
        #region Controller Lifecycle

        protected IOUserController(IConfiguration configuration, TDBContext databaseContext, IWebHostEnvironment environment, ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }

        #endregion

        #region User Methods

        [IOUserRole(UserRoles.Admin)]
        [HttpPost]
        public virtual IOAddUserResponseModel AddUser([FromBody] IOAddUserRequestModel requestModel) 
        {
			// Validate request
			if (requestModel == null
                || String.IsNullOrEmpty(requestModel.UserName)
                || String.IsNullOrEmpty(requestModel.Password)
                || requestModel.Password.Length < 4)
			{
				// Then return validation error
                return new IOAddUserResponseModel(IOResponseStatusMessages.BAD_REQUEST);
			}

            // Obtain add user response
            Tuple<bool, int, string> addUserStatus = ViewModel.AddUser(requestModel.UserName, requestModel.Password, requestModel.UserRole);

            // Check add user is success
            if (addUserStatus.Item1) 
            {
				// Create and return response
                return new IOAddUserResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), addUserStatus.Item2, addUserStatus.Item3);
            }

			// Return user exists response
            return new IOAddUserResponseModel(IOResponseStatusMessages.USER_EXISTS);
		}

        [IOUserRole(UserRoles.User)]
		[HttpPost]
        public IOResponseModel ChangePassword([FromBody] IOUserChangePasswordRequestModel requestModel) 
        {
			// Validate request
			if (requestModel == null
                || String.IsNullOrEmpty(requestModel.UserName)
                || String.IsNullOrEmpty(requestModel.NewPassword)
                || requestModel.NewPassword.Length < 4)
			{
				// Then return validation error
                return new IOAddUserResponseModel(IOResponseStatusMessages.BAD_REQUEST);
			}

            // Check change password is success
            if (ViewModel.ChangePassword(requestModel.UserName, requestModel.OldPassword, requestModel.NewPassword))
            {
				// Return response
				return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK));
            }

            // Return response
            return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.BAD_REQUEST, "Invalid user."));
        }

        [IOUserRole(UserRoles.Admin)]
        [HttpPost]
		public virtual IOResponseModel DeleteUser([FromBody] IODeleteUserRequestModel requestModel) 
        {
			// Validate request
			if (requestModel == null)
			{
				// Then return validation error
                return new IOAddUserResponseModel(IOResponseStatusMessages.BAD_REQUEST);
			}

            // Obtain user entity
            IOUserEntity userEntity = DatabaseContext.Users.Find(requestModel.UserId);
            int currentUserRole = ViewModel.UserEntity.UserRole;

			// Check user entity is not null
            if (userEntity != null && currentUserRole <= userEntity.UserRole)
			{
                // Delete all entity
                DatabaseContext.Remove(userEntity);
                DatabaseContext.SaveChanges();

				// Then return response
				return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK));
			}

			// Return bad request
            throw new IOUserNotFoundException();
        }

        [IOUserRole(UserRoles.Admin)]
        [HttpGet]
        public virtual IOListUserResponseModel ListUsers() 
        {
            // Obtain user list
            List<IOUserInfoModel> users = ViewModel.ListUsers();

			// Create and return response
			return new IOListUserResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), users);
        }

        [IOUserRole(UserRoles.Admin)]
        [HttpPost]
        public IOUpdateUserResponseModel UpdateUser([FromBody] IOUpdateUserRequestModel requestModel)
        {
            // Validate request
            if (requestModel == null)
            {
                // Then return validation error
                return new IOUpdateUserResponseModel(IOResponseStatusMessages.BAD_REQUEST);
            }

            int status = ViewModel.UpdateUser(requestModel);

            if (status == 3)
            {
                // Obtain 400 error 
                return new IOUpdateUserResponseModel(IOResponseStatusMessages.USER_EXISTS);
            }

            return new IOUpdateUserResponseModel(IOResponseStatusMessages.OK);
        }

        #endregion

    }
}
