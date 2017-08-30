using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Entities.AutoIncrements;
using IOBootstrap.NET.Common.Entities.Users;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.WebApi.BackOffice.Controllers;
using IOBootstrap.NET.WebApi.User.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Realms;
using System;
using System.Linq;
using System.Collections.Generic;

namespace IOBootstrap.NET.WebApi.User.Controllers
{
    public abstract class IOUserController<TLogger> : IOBackOfficeController<TLogger>
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
			if (!this.CheckClient(requestModel.ClientInfo))
			{
				// Then return invalid clients
				this.Response.StatusCode = 400;
                return new IOAddUserResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.INVALID_CLIENTS), 0, null);
			}

			// Obtain realm instance
			Realm realm = _database.GetRealmForMainThread();

            // Obtain users entity
            var usersEntities = realm.All<IOUserEntity>()
                                                 .Where((arg) => arg.UserName == requestModel.UserName);

			// Check push notification entity exists
			if (usersEntities.Count() > 0)
			{
				// Dispose realm
				realm.Dispose();

                // Return user exists response
                this.Response.StatusCode = 400;
                return new IOAddUserResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.BAD_REQUEST, "User exists"), 0, null);
			}

            // Generate user id
            int userId = IOAutoIncrementsEntity.IdForClass(_database, typeof(IOUserEntity));

            // Create a users entity 
            IOUserEntity userEntity = new IOUserEntity()
            {
                ID = userId,
                UserName = requestModel.UserName.ToLower(),
                Password = IOCommonHelpers.HashPassword(requestModel.Password),
                UserRole = requestModel.UserRole,
                UserToken = null,
                TokenDate = DateTime.UtcNow
            };

			// Write user to database
			_database.InsertEntity(userEntity)
					 .Subscribe();

			// Dispose realm
			realm.Dispose();

			// Create and return response
            return new IOAddUserResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), userId, requestModel.UserName);
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
			if (!this.CheckClient(requestModel.ClientInfo))
			{
				// Then return invalid clients
				this.Response.StatusCode = 400;
				return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.INVALID_CLIENTS));
			}

			// Obtain realm instance 
            Realm realm = _database.GetRealmForThread();

            // Obtain user entity
            var userEntities = realm.All<IOUserEntity>()
                                           .Where((arg1) => arg1.UserName == requestModel.UserName.ToLower());

            // Check user finded
            if (userEntities.Count() > 0) {
                // Obtain user entity
                IOUserEntity user = userEntities.First();

                // Check user old password is valid
                if (IOCommonHelpers.VerifyPassword(requestModel.OldPassword, user.Password)) {
					// Begin write transaction
					Transaction realmTransaction = realm.BeginWrite();

                    // Update user password properties
                    user.Password = requestModel.NewPassword;
                    user.UserToken = null;

                    // Update user password
                    realm.Add(user, true);

                    // Commit transaction
                    realmTransaction.Commit();

					// Dispose realm
					realm.Dispose();

                    // Return response
                    return new IOResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK));
				}
            }

            // Dispose realm
            realm.Dispose();

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
			if (!this.CheckClient(requestModel.ClientInfo))
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
			if (!this.CheckClient(requestModel.ClientInfo))
			{
				// Then return invalid clients
				this.Response.StatusCode = 400;
                return new IOListUserResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.INVALID_CLIENTS), null);
			}

			// Create list for clients
            List<IOUserInfoModel> users = new List<IOUserInfoModel>();

			// Obtain realm 
            Realm realm = _database.GetRealmForMainThread();

			// Check real is not null
			if (realm != null)
			{
				// Obtain users from realm
                var user = realm.All<IOUserEntity>();

				// Check users is not null
				if (user != null)
				{
					// Loop throught clients
					for (int i = 0; i < user.Count(); i++)
					{
						// Obtain users entity
                        IOUserEntity userEntity = user.ElementAt(i);

						// Create user info model
                        IOUserInfoModel model = new IOUserInfoModel() {
                            ID = userEntity.ID,
                            UserName = userEntity.UserName,
                            UserRole = userEntity.UserRole,
                            UserToken = userEntity.UserToken,
                            TokenDate = userEntity.TokenDate
                        };

						// Add model to user list
						users.Add(model);
					}
				}
			}

			// Dispose realm
			realm.Dispose();

			// Create and return response
			return new IOListUserResponseModel(new IOResponseStatusModel(IOResponseStatusMessages.OK), users);
        }

        #endregion

    }
}
