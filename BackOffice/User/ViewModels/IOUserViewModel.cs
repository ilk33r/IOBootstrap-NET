using System;
using IOBootstrap.NET.Common.Messages.MW;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Exceptions.Members;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.Users;
using IOBootstrap.NET.Common.Models.Users;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.ViewModels;

namespace IOBootstrap.NET.BackOffice.User.ViewModels
{
    public class IOUserViewModel : IOBackOfficeViewModel
    {

        #region Initialization Methods

        public IOUserViewModel() : base()
        {
        }

        #endregion

        #region View Model Methods

        public virtual IOAddUserResponseModel AddUser(IOAddUserRequestModel requestModel)
        {
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackofficeUserControllerNameKey);
            requestModel.Password = IOPasswordUtilities.HashPassword(requestModel.Password);
            IOAddUserResponseModel response = MWConnector.Get<IOAddUserResponseModel>(controller + "/" + "AddUser", requestModel);

            // Check user added
            MWConnector.HandleResponse(response, code => {
                if (code == 500)
                {
                    // Return user exists response
                    throw new IOUserExistsException();
                }
            });

            // Return response
            return response;
        }

        public virtual void ChangePassword(string userName, string oldPassword, string newPassword) 
        {
            string authController = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeAuthenticationControllerNameKey);
            IOMWFindRequestModel userRequestModel = new IOMWFindRequestModel()
            {
                Where = userName
            };
            IOMWUserResponseModel currentUser = MWConnector.Get<IOMWUserResponseModel>(authController + "/" + "FindUserFromName", userRequestModel);
            MWConnector.HandleResponse(currentUser, code => {
                // Return user exists response
                throw new IOUserNotFoundException();
            });

            // Check user old password is valid
            if (((UserRoles)UserModel.UserRole == UserRoles.SuperAdmin) || IOPasswordUtilities.VerifyPassword(oldPassword, currentUser.Password))
			{
                string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackofficeUserControllerNameKey);
                IOMWFindRequestModel changePasswordRequest = new IOMWFindRequestModel()
                {
                    ID = currentUser.ID,
                    Where = IOPasswordUtilities.HashPassword(newPassword)
                };
                IOResponseModel response = MWConnector.Get<IOResponseModel>(controller + "/" + "ChangePassword", changePasswordRequest);
                MWConnector.HandleResponse(response, code => {
                    // Return user exists response
                    throw new IOUserNotFoundException();
                });

                return;
            }

            // Return response
            throw new IOInvalidPermissionException();
        }
        
        public virtual IList<IOUserInfoModel> ListUsers()
        {
			string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackofficeUserControllerNameKey);
            IOMWFindRequestModel requestModel = new IOMWFindRequestModel();
            IOMWListResponseModel<IOUserInfoModel> response = MWConnector.Get<IOMWListResponseModel<IOUserInfoModel>>(controller + "/" + "ListUsers", requestModel);

            if (MWConnector.HandleResponse(response, code => {}))
            {
                return response.Items;
            }

            return new List<IOUserInfoModel>();
        }

        public virtual void UpdateUser(IOUpdateUserRequestModel request)
        {
            if (!IOUserRoleUtility.CheckRole(UserRoles.Admin, (UserRoles)UserModel.UserRole))
            {
                throw new IOInvalidPermissionException();
            }

            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackofficeUserControllerNameKey);
            if (!String.IsNullOrEmpty(request.UserPassword))
            {
                request.UserPassword = IOPasswordUtilities.HashPassword(request.UserPassword);
            }
            IOResponseModel response = MWConnector.Get<IOResponseModel>(controller + "/" + "UpdateUser", request);
            MWConnector.HandleResponse(response, code => {
                if (code == 500)
                {
                    throw new IOUserNotFoundException();
                }

                if (code == 501)
                {
                    throw new IOUserExistsException();
                }
            });
        }

        public virtual void DeleteUser(IODeleteUserRequestModel request)
        {
            string authController = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeAuthenticationControllerNameKey);
            IOMWFindRequestModel userRequestModel = new IOMWFindRequestModel()
            {
                ID = request.UserId
            };
            IOMWUserResponseModel userEntity = MWConnector.Get<IOMWUserResponseModel>(authController + "/" + "FindUserById", userRequestModel);
            MWConnector.HandleResponse(userEntity, code => {
                throw new IOUserNotFoundException();
            });

			// Check user entity is not null
            if (UserModel.UserRole <= userEntity.UserRole)
			{
                string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackofficeUserControllerNameKey);
                IOResponseModel response = MWConnector.Get<IOResponseModel>(controller + "/" + "DeleteUser", request);
                MWConnector.HandleResponse(userEntity, code => {
                    throw new IOUserNotFoundException();
                });

                return;
			}

            throw new IOInvalidPermissionException();
        }

        #endregion
    }
}
