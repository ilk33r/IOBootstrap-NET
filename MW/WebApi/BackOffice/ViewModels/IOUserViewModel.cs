using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Messages.Users;
using IOBootstrap.NET.Common.Models.Users;
using IOBootstrap.NET.MW.Core.ViewModels;
using IOBootstrap.NET.MW.DataAccess.Context;
using IOBootstrap.NET.MW.DataAccess.Entities;

namespace IOBootstrap.NET.MW.WebApi.BackOffice.ViewModels
{
    public class IOUserViewModel<TDBContext> : IOMWViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {
        public virtual IOAddUserResponseModel AddUser(string userName, string password, int userRole)
        {
            // Obtain users entity
            IOUserEntity user = DatabaseContext.Users
                                                .Where(u => u.UserName.Equals(userName))
                                                .FirstOrDefault();

			// Check push notification entity exists
            if (user != null)
			{
                // Return user exists response
                return null;
			}

			// Create a users entity 
			IOUserEntity newUserEntity = new IOUserEntity()
			{
				UserName = userName.ToLower(),
                Password = password,
				UserRole = userRole,
				UserToken = null,
				TokenDate = DateTime.UtcNow
			};

            // Write user to database
            DatabaseContext.Add(newUserEntity);
            DatabaseContext.SaveChanges();

            // Return status
            return new IOAddUserResponseModel(newUserEntity.ID, userName);
        }

        public bool ChangePassword(int id, string newPassword) 
        {
            // Obtain user entity
            IOUserEntity user = DatabaseContext.Users.Find(id);

            // Check user found
            if (user == null)
            {
                return false;
            }

            // Update user password properties
            user.Password = newPassword;
			user.UserToken = null;

            // Update user password
            DatabaseContext.Update(user);
            DatabaseContext.SaveChanges();

            return true;
        }

        public virtual IList<IOUserInfoModel> ListUsers()
        {
            // Obtain users from db
            return DatabaseContext.Users
                                    .Select(u => new IOUserInfoModel()
                                    {
                                        ID = u.ID,
                                        UserName = u.UserName,
                                        UserRole = u.UserRole,
                                        UserToken = u.UserToken,
                                        TokenDate = u.TokenDate
                                    })
                                    .OrderBy(u => u.ID)
                                    .ToList();
        }

        public int UpdateUser(IOUpdateUserRequestModel request)
        {
            IOUserEntity user = DatabaseContext.Users.Find(request.UserId);
            string userName = request.UserName.ToLower();

            if (user == null)
            {
                return 500;
            }

            var newUsers = DatabaseContext.Users.Where((arg) => arg.UserName == userName && arg.UserName != user.UserName);
            if (newUsers == null || newUsers.Count() != 0)
            {
                return 501;
            }

            // Update user properties
            user.UserName = userName;
            user.UserRole = request.UserRole;

            if (!String.IsNullOrEmpty(request.UserPassword))
            {
                user.Password = request.UserPassword;
                user.UserToken = null;
            }

            // Update user password
            DatabaseContext.Update(user);
            DatabaseContext.SaveChanges();

            return IOResponseStatusMessages.OK;
        }

        public int DeleteUser(IODeleteUserRequestModel request)
        {
            IOUserEntity user = DatabaseContext.Users.Find(request.UserId);

            if (user == null)
            {
                return IOResponseStatusMessages.UnkownException;
            }

            // Update user password
            DatabaseContext.Remove(user);
            DatabaseContext.SaveChanges();

            return IOResponseStatusMessages.OK;
        }
    }
}
