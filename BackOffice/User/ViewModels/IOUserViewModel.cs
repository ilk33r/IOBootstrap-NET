using System;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Exceptions.Members;
using IOBootstrap.NET.Common.Messages.Users;
using IOBootstrap.NET.Common.Models.Users;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.BackOffice.User.Interfaces;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.BackOffice.User.ViewModels
{
    public class IOUserViewModel<TDBContext> : IOBackOfficeViewModel<TDBContext>, IIOUserViewModel<TDBContext>
    where TDBContext : IODatabaseContext<TDBContext> 
    {

        #region Initialization Methods

        public IOUserViewModel() : base()
        {
        }

        #endregion

        #region View Model Methods

        public virtual IOAddUserResponseModel AddUser(IOAddUserRequestModel requestModel)
        {
            // Obtain users entity
            IOUserEntity user = DatabaseContext.Users
                                                .Where(u => u.UserName.Equals(requestModel.UserName))
                                                .FirstOrDefault();

			// Check push notification entity exists
            if (user != null)
			{
                // Return user exists response
                throw new IOUserExistsException();
			}

			// Create a users entity 
			IOUserEntity newUserEntity = new IOUserEntity()
			{
				UserName = requestModel.UserName.ToLower(),
                Password = IOPasswordUtilities.HashPassword(requestModel.Password),
				UserRole = requestModel.UserRole,
				UserToken = null,
				TokenDate = DateTime.UtcNow
			};

            // Write user to database
            DatabaseContext.Add(newUserEntity);
            DatabaseContext.SaveChanges();

            // Return status
            return new IOAddUserResponseModel(newUserEntity.ID, requestModel.UserName);
        }

        public virtual void ChangePassword(string userName, string oldPassword, string newPassword)
        {
            IOUserEntity currentUser = DatabaseContext.Users
                                                        .Where(u => u.UserName.Equals(userName))
                                                        .FirstOrDefault();

            if (currentUser == null)
            {
                // Return user exists response
                throw new IOUserNotFoundException();
            }

            // Check user old password is valid
            if (((UserRoles)UserModel.UserRole == UserRoles.SuperAdmin) || IOPasswordUtilities.VerifyPassword(oldPassword, currentUser.Password))
			{
                // Update user password properties
                currentUser.Password = IOPasswordUtilities.HashPassword(newPassword);
			    currentUser.UserToken = null;

                // Update user password
                DatabaseContext.Update(currentUser);
                DatabaseContext.SaveChanges();

                return;
            }

            // Return response
            throw new IOInvalidPermissionException();
        }
        
        public virtual IList<IOUserInfoModel> ListUsers()
        {
            IList<IOUserInfoModel> userList = DatabaseContext.Users
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

            if (userList == null)
            {
                return new List<IOUserInfoModel>();
            }

            return userList;
        }

        public virtual void UpdateUser(IOUpdateUserRequestModel request)
        {
            if (!IOUserRoleUtility.CheckRole(UserRoles.Admin, (UserRoles)UserModel.UserRole))
            {
                throw new IOInvalidPermissionException();
            }

            IOUserEntity user = DatabaseContext.Users.Find(request.UserId);
            string userName = request.UserName.ToLower();

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

        public virtual void DeleteUser(IODeleteUserRequestModel request)
        {
            IOUserEntity user = DatabaseContext.Users.Find(request.UserId);

            if (user == null)
            {
                throw new IOUserNotFoundException();
            }

            // Check user entity is not null
            if (UserModel.UserRole <= user.UserRole)
			{
                // Update user password
                DatabaseContext.Remove(user);
                DatabaseContext.SaveChanges();

                return;
            }

            throw new IOInvalidPermissionException();
        }

        #endregion
    }
}
