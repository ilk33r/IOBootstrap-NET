using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Core.Extensions;
using IOBootstrap.NET.Core.Interfaces;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.Common.Models.Users;

namespace IOBootstrap.NET.Core.ViewModels;

public abstract class IOBackOfficeViewModel<TDBContext> : IOViewModel<TDBContext>, IIOBackOfficeViewModel<TDBContext>, IIOUserCredential<TDBContext>
where TDBContext : IOBaseDatabaseContext<TDBContext>
{

    #region Publics

    public IOUserInfoModel? UserModel { get; set; }

    #endregion

    #region Initialization Methods

    public IOBackOfficeViewModel() : base()
    {
    }

    #endregion

    #region View Model Methods

    public virtual bool IsBackOffice()
    {
        return this.CheckHasUserTokenAndIsValid(Request);
    }

    #endregion

    #region Helper Methods

    public override int GetUserRole()
    {
        // Check user exists
        if (UserModel != null)
        {
            // Return role
            return UserModel.UserRole;
        }

        return (int)UserRoles.AnonmyMouse;
    }

    #endregion

}
