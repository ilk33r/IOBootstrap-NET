using System;
using System.Linq;
using System.Collections.Generic;
using IOBootstrap.NET.DataAccess.Entities;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.Common.Models.Users;
using IOBootstrap.NET.Common.Messages.Users;
using IOBootstrap.NET.Common.Exceptions.Members;
using IOBootstrap.NET.Common.Exceptions.Common;

namespace IOBootstrap.NET.BackOffice.User.ViewModels
{
    public class IOUserViewModel<TDBContext> : IOBackOfficeViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Initialization Methods

        public IOUserViewModel() : base()
        {
        }

        #endregion

        #region View Model Methods

        public virtual Tuple<int, string> AddUser(string userName, string password, int userRole)
        {
            // Obtain users entity
            IOUserEntity user = IOUserEntity.FindUserFromName(DatabaseContext.Users, userName);

			// Check push notification entity exists
            if (user != null)
			{
                // Return user exists response
                throw new IOUserExistsException();
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
            DatabaseContext.Add(newUserEntity);
            DatabaseContext.SaveChanges();

            // Return status
            return new Tuple<int, string>(newUserEntity.ID, userName);
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword) 
        {
            // Obtain user entity
            IOUserEntity user = IOUserEntity.FindUserFromName(DatabaseContext.Users, userName);

            // Check user old password is valid
            if (user != null && ((UserRoles)UserEntity.UserRole == UserRoles.SuperAdmin || IOPasswordUtilities.VerifyPassword(oldPassword, user.Password)))
			{
				// Update user password properties
                user.Password = IOPasswordUtilities.HashPassword(newPassword);
			    user.UserToken = null;

                // Update user password
                DatabaseContext.Update(user);
                DatabaseContext.SaveChanges();

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
            var user = DatabaseContext.Users;

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

        public void UpdateUser(IOUpdateUserRequestModel request)
        {
            IOUserEntity user = DatabaseContext.Users.Find(request.UserId);
            string userName = request.UserName.ToLower();

            if (!IOUserRoleUtility.CheckRole(UserRoles.Admin, (UserRoles)UserEntity.UserRole))
            {
                throw new IOInvalidPermissionException();
            }

            if (user == null)
            {
                throw new IOUserNotFoundException();
            }

            var newUsers = DatabaseContext.Users.Where((arg) => arg.UserName == userName && arg.UserName != user.UserName);
            if (newUsers == null || newUsers.Count() != 0)
            {
                throw new IOUserExistsException();
            }

            // Update user properties
            user.UserName = userName;
            user.UserRole = request.UserRole;

            if (!String.IsNullOrEmpty(request.UserPassword))
            {
                user.Password = IOPasswordUtilities.HashPassword(request.UserPassword);
                user.UserToken = null;
            }

            // Update user password
            DatabaseContext.Update(user);
            DatabaseContext.SaveChanges();
        }

        #endregion
    }
}
