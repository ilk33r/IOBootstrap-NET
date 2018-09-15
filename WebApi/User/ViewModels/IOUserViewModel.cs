using System;
using System.Linq;
using System.Collections.Generic;
using IOBootstrap.NET.Common.Entities.Users;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.WebApi.User.Models;
using IOBootstrap.NET.Common.Enumerations;

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

        public virtual Tuple<bool, int, string> AddUser(string userName, string password, int userRole)
        {
            // Obtain users entity
            IOUserEntity user = IOUserEntity.FindUserFromName(_databaseContext.Users, userName);

			// Check push notification entity exists
            if (user != null)
			{
                // Return user exists response
                return new Tuple<bool, int, string>(false, 0, null);
			}

			// Create a users entity 
			IOUserEntity newUserEntity = new IOUserEntity()
			{
				UserName = userName.ToLower(),
                Password = IOPasswordUtilities.HashPassword(password),
				UserRole = userRole,
				UserToken = null,
				TokenDate = DateTime.UtcNow
			};

            // Write user to database
            _databaseContext.Add(newUserEntity);
            _databaseContext.SaveChanges();

            // Return status
            return new Tuple<bool, int, string>(true, userEntity.ID, userName);
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword) 
        {
            // Obtain user entity
            IOUserEntity user = IOUserEntity.FindUserFromName(_databaseContext.Users, userName);

            // Check user old password is valid
            if (user != null && ((UserRoles)this.userEntity.UserRole == UserRoles.SuperAdmin || IOPasswordUtilities.VerifyPassword(oldPassword, user.Password)))
			{
				// Update user password properties
                user.Password = IOPasswordUtilities.HashPassword(newPassword);
			    user.UserToken = null;

                // Update user password
                _databaseContext.Update(user);
                _databaseContext.SaveChanges();

                // Return response
                return true;
			}

            // Return response
            return false;
        }

        public virtual List<IOUserInfoModel> ListUsers()
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
					IOUserEntity currentUserEntity = user.Skip(i).First();

					// Create user info model
					IOUserInfoModel model = new IOUserInfoModel()
					{
                        ID = currentUserEntity.ID,
                        UserName = currentUserEntity.UserName,
                        UserRole = currentUserEntity.UserRole,
                        UserToken = currentUserEntity.UserToken,
                        TokenDate = currentUserEntity.TokenDate
					};

					// Add model to user list
					users.Add(model);
				}
			}

            // Return users
            return users;
        }

        public int UpdateUser(IOUpdateUserRequestModel request)
        {
            IOUserEntity user = _databaseContext.Users.Find(request.UserId);
            string userName = request.UserName.ToLower();

            if (!UserRoleUtility.CheckRole(UserRoles.Admin, (UserRoles)this.userEntity.UserRole))
            {
                return 2;
            }

            if (user != null) 
            {
                var newUsers = _databaseContext.Users.Where((arg) => arg.UserName == userName && arg.UserName != user.UserName);
                if (newUsers == null || newUsers.Count() != 0)
                {
                    return 3;
                }

                // Update user properties
                user.UserName = userName;
                user.UserRole = request.UserRole;

                if (String.IsNullOrEmpty(request.UserPassword))
                {
                    user.Password = IOPasswordUtilities.HashPassword(request.UserPassword);
                    user.UserToken = null;
                }

                // Update user password
                _databaseContext.Update(user);
                _databaseContext.SaveChanges();

                return 0;
            }

            return 1;
        }

        #endregion
    }
}
