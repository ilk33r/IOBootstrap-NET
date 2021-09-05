using System;
using System.Collections.Generic;
using IOBootstrap.NET.BackOffice.User.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Exceptions.Members;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.Users;
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
    [IOBackoffice]
    public abstract class IOUserController<TViewModel, TDBContext> : IOBackOfficeController<TViewModel, TDBContext> where TViewModel : IOUserViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {
        #region Controller Lifecycle

        protected IOUserController(IConfiguration configuration, TDBContext databaseContext, IWebHostEnvironment environment, ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }

        #endregion

        #region User Methods

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.Admin)]
        [HttpPost]
        public virtual IOAddUserResponseModel AddUser([FromBody] IOAddUserRequestModel requestModel) 
        {
            // Obtain add user response
            Tuple<int, string> addUserStatus = ViewModel.AddUser(requestModel.UserName, requestModel.Password, requestModel.UserRole);

			// Create and return response
            return new IOAddUserResponseModel(addUserStatus.Item1, addUserStatus.Item2);
		}

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.User)]
		[HttpPost]
        public IOResponseModel ChangePassword([FromBody] IOUserChangePasswordRequestModel requestModel) 
        {
            // Check change password is success
            ViewModel.ChangePassword(requestModel.UserName, requestModel.OldPassword, requestModel.NewPassword);

			// Return response
			return new IOResponseModel();
        }

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.Admin)]
        [HttpPost]
		public virtual IOResponseModel DeleteUser([FromBody] IODeleteUserRequestModel requestModel) 
        {
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
				return new IOResponseModel();
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
			return new IOListUserResponseModel(users);
        }

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.Admin)]
        [HttpPost]
        public IOUpdateUserResponseModel UpdateUser([FromBody] IOUpdateUserRequestModel requestModel)
        {
            ViewModel.UpdateUser(requestModel);
            return new IOUpdateUserResponseModel();
        }

        #endregion

    }
}
