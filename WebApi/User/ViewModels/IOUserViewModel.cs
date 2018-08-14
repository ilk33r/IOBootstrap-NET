﻿using IOBootstrap.NET.Common.Entities.Users;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.WebApi.BackOffice.ViewModels;
using IOBootstrap.NET.WebApi.User.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace IOBootstrap.NET.WebApi.User.ViewModels
{
    public class IOUserViewModel<TDBContext> : IOBackOfficeViewModel<TDBContext>
        where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Initialization Methods

        public IOUserViewModel() : base()
        {
        }

        #endregion

        #region View Model Methods

        public Tuple<bool, int, string> AddUser(string userName, string password, int userRole)
        {
			// Obtain users entity
            var usersEntities = _databaseContext.Users.Where((arg) => arg.UserName == userName);

			// Check push notification entity exists
			if (usersEntities.Count() > 0)
			{
                // Return user exists response
                return new Tuple<bool, int, string>(false, 0, null);
			}

			// Create a users entity 
			IOUserEntity userEntity = new IOUserEntity()
			{
				UserName = userName.ToLower(),
				Password = IOCommonHelpers.HashPassword(password),
				UserRole = userRole,
				UserToken = null,
				TokenDate = DateTime.UtcNow
			};

            // Write user to database
            _databaseContext.Add(userEntity);
            _databaseContext.SaveChangesAsync();

            // Return status
            return new Tuple<bool, int, string>(true, userEntity.ID, userName);
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword) 
        {
			// Obtain user entity
            var userEntities = _databaseContext.Users.Where((arg1) => arg1.UserName == userName.ToLower());

			// Check user finded
			if (userEntities.Count() > 0)
			{
				// Obtain user entity
				IOUserEntity user = userEntities.First();

				// Check user old password is valid
				if (IOCommonHelpers.VerifyPassword(oldPassword, user.Password))
				{
					// Update user password properties
                    user.Password = IOCommonHelpers.HashPassword(newPassword);
					user.UserToken = null;

                    // Update user password
                    _databaseContext.Update(user);
                    _databaseContext.SaveChangesAsync();

                    // Return response
                    return true;
				}
			}

            // Return response
            return false;
        }

        public List<IOUserInfoModel> ListUsers()
        {
			// Create list for clients
			List<IOUserInfoModel> users = new List<IOUserInfoModel>();

            // Obtain users from realm
            var user = _databaseContext.Users;

			// Check users is not null
			if (user != null)
			{
				// Loop throught clients
				for (int i = 0; i < user.Count(); i++)
				{
					// Obtain users entity
					IOUserEntity userEntity = user.Skip(i).First();

					// Create user info model
					IOUserInfoModel model = new IOUserInfoModel()
					{
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

            // Return users
            return users;
        }

        #endregion
    }
}
