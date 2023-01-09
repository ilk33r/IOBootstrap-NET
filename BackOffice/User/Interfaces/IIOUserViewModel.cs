using IOBootstrap.NET.Common.Messages.Users;
using IOBootstrap.NET.Common.Models.Users;
using IOBootstrap.NET.Core.Interfaces;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.BackOffice.User.Interfaces;

public interface IIOUserViewModel<TDBContext> : IIOBackOfficeViewModel<TDBContext>
where TDBContext : IODatabaseContext<TDBContext> 
{
    public IOAddUserResponseModel AddUser(IOAddUserRequestModel requestModel);
    public void ChangePassword(string userName, string oldPassword, string newPassword);
    public IList<IOUserInfoModel> ListUsers();
    public void UpdateUser(IOUpdateUserRequestModel request);
    public void DeleteUser(IODeleteUserRequestModel request);
}
