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
using IOBootstrap.NET.Core.Extensions;
using IOBootstrap.NET.Core.Interfaces;

namespace IOBootstrap.NET.BackOffice.User.ViewModels;

public class IOUserViewModel<TDBContext> : IOBackOfficeViewModel<TDBContext>, IIOUserViewModel<TDBContext>, IIOUserCredential<TDBContext>
where TDBContext : IODatabaseContext<TDBContext>
{

    #region Initialization Methods

    public IOUserViewModel() : base()
    {
    }

    #endregion

    #region View Model Methods

    public virtual async Task<IOAddUserResponseModel> AddUser(IOAddUserRequestModel requestModel)
    {
        // Obtain users entity
        IOUserEntity? user = DatabaseContext.Users
                                            .Where(u => u.UserName!.Equals(requestModel.UserName))
                                            .FirstOrDefault();

        // Check push notification entity exists
        if (user != null)
        {
            // Return user exists response
            throw new IOUserExistsException();
        }

        string decryptedPassword = await DecryptString(requestModel.Password ?? "");
        IOUserEntity? newUserEntity = null;

        using (var passwordUtilities = new IOPasswordUtilities())
        {
            await passwordUtilities.HashPassword(decryptedPassword, hashed =>
            {
                // Create a users entity 
                newUserEntity = new IOUserEntity()
                {
                    UserName = requestModel.UserName!.ToLower(),
                    Password = hashed,
                    UserRole = requestModel.UserRole,
                    UserToken = null,
                    TokenDate = DateTimeOffset.UtcNow,
                    IsActive = requestModel.IsActive ?? false,
                    ActivationEndDate = requestModel.ActivationEndDate ?? DateTimeOffset.UtcNow,
                    CreatedBy = UserModel?.UserName ?? string.Empty,
                    CreatedDate = DateTimeOffset.UtcNow,
                    UpdateDate = DateTimeOffset.UtcNow,
                    WrongPasswordAttemptCount = 0,
                    PasswordExpireDate = DateTimeOffset.UtcNow.AddYears(-1),
                    LastWrongPasswordAttemptDate = new DateTimeOffset()
                };

                // Write user to database
                DatabaseContext.Add(newUserEntity);
                DatabaseContext.SaveChanges();
            });
        }

        // Return status
        return new IOAddUserResponseModel(newUserEntity?.ID ?? 0, requestModel.UserName ?? "");
    }

    public virtual async Task ChangePassword(string oldPassword, string newPassword)
    {
        await this.ChangeUserPassword(oldPassword, newPassword);
    }

    public virtual async Task ResetPassword(string userName, string newPassword)
    {
        IOUserEntity? currentUser = DatabaseContext.Users
                                                    .Where(u => u.UserName!.Equals(userName))
                                                    .FirstOrDefault();

        if (currentUser == null)
        {
            // Return user exists response
            throw new IOUserNotFoundException();
        }

        if (currentUser.UserRole == (int)UserRoles.SuperAdmin && GetUserRole() > (int)UserRoles.SuperAdmin)
        {
            throw new IOInvalidCredentialsException("You can not reset this user password.");
        }

        if (currentUser.UserRole != (int)UserRoles.SuperAdmin && GetUserRole() > currentUser.UserRole)
        {
            throw new IOInvalidCredentialsException("You can not reset this user password.");
        }

        using (var passwordUtilities = new IOPasswordUtilities())
        {
            string decryptedNewPassword = await DecryptString(newPassword);
            await passwordUtilities.HashPassword(decryptedNewPassword, hashed =>
            {
                // Update user password properties
                currentUser.Password = hashed;
                currentUser.UserToken = null;
                currentUser.UpdateDate = DateTimeOffset.UtcNow;
                currentUser.WrongPasswordAttemptCount = 0;
                currentUser.LastWrongPasswordAttemptDate = new DateTimeOffset();
                currentUser.PasswordExpireDate = DateTimeOffset.UtcNow.AddYears(-1);

                // Update user password
                DatabaseContext.Update(currentUser);
                DatabaseContext.SaveChanges();
            });
        }
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
                                                                TokenDate = u.TokenDate,
                                                                IsActive = u.IsActive,
                                                                ActivationEndDate = u.ActivationEndDate,
                                                                CreatedBy = u.CreatedBy,
                                                                CreatedDate = u.CreatedDate,
                                                                UpdateDate = u.UpdateDate
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
        UserRoles currentUserRole = ((UserRoles?)UserModel?.UserRole) ?? UserRoles.AnonmyMouse;
        if (!IOUserRoleUtility.CheckRole(UserRoles.Admin, currentUserRole))
        {
            throw new IOInvalidPermissionException();
        }

        IOUserEntity? user = DatabaseContext.Users.Find(request.UserId);
        string userName = request.UserName?.ToLower() ?? "";

        if (user == null)
        {
            throw new IOUserNotFoundException();
        }

        var newUsers = DatabaseContext.Users.Where((arg) => arg.UserName == userName && arg.UserName != user.UserName);
        if (newUsers == null || newUsers.Count() != 0)
        {
            throw new IOUserExistsException();
        }

        if (user.UserRole != (int)UserRoles.SuperAdmin && GetUserRole() > user.UserRole)
        {
            throw new IOInvalidCredentialsException("You can not edit this user.");
        }

        // Update user properties
        user.UserName = userName;
        user.UserRole = request.UserRole ?? 999;
        user.IsActive = request.IsActive ?? false;
        user.ActivationEndDate = request.ActivationEndDate ?? DateTimeOffset.UtcNow;
        user.UpdateDate = DateTimeOffset.UtcNow;

        // Update user password
        DatabaseContext.Update(user);
        DatabaseContext.SaveChanges();
    }

    public virtual void DeleteUser(IODeleteUserRequestModel request)
    {
        IOUserEntity? user = DatabaseContext.Users.Find(request.UserId);

        if (user == null)
        {
            throw new IOUserNotFoundException();
        }

        if (user.UserRole != (int)UserRoles.SuperAdmin && GetUserRole() > user.UserRole)
        {
            throw new IOInvalidCredentialsException("You can not delete this user.");
        }

        // Check user entity is not null
        if ((UserModel?.UserRole ?? (int)UserRoles.AnonmyMouse) <= user.UserRole)
        {
            // Update user password
            DatabaseContext.Remove(user);
            DatabaseContext.SaveChanges();

            return;
        }

        throw new IOInvalidPermissionException();
    }

    public virtual async Task Logout(string userName)
    {
        await this.LogoutUser(userName);
    }

    #endregion
}
