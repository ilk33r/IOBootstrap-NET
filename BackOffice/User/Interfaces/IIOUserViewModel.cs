using IOBootstrap.NET.Common.Messages.Users;
using IOBootstrap.NET.Core.Interfaces;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.BackOffice.User.Interfaces;

public interface IIOUserViewModel<TDBContext> : IIOBackOfficeViewModel<TDBContext>
where TDBContext : IOBaseDatabaseContext<TDBContext> 
{
    public Task<IOAddUserResponseModel> AddUser(IOAddUserRequestModel requestModel);
    public Task ChangePassword(string oldPassword, string newPassword);
    public Task ResetPassword(string userName, string newPassword);
    public IOListUserResponseModel ListUsers(int? start, int? count);
    public void UpdateUser(IOUpdateUserRequestModel request);
    public void DeleteUser(IODeleteUserRequestModel request);
    public Task Logout(string userName);
}
