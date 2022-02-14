using System;
using IOBootstrap.Net.Common.Messages.Users;
using IOBootstrap.NET.MW.Core.ViewModels;
using IOBootstrap.NET.MW.DataAccess.Context;

namespace IOBootstrap.NET.MW.WebApi.BackOffice.ViewModels
{
    public class IOBackOfficeAuthenticationViewModel<TDBContext> : IOMWViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {

        public IOMWUserResponseModel? FindUserById(int id)
        {
            return DatabaseContext.Users
                                    .Select(u => new IOMWUserResponseModel()
                                    {
                                        ID = u.ID,
                                        UserName = u.UserName,
                                        UserRole = u.UserRole,
                                        UserToken = u.UserToken,
                                        TokenDate = u.TokenDate
                                    })
                                    .Where(u => u.ID == id)
                                    .FirstOrDefault();
        }
    }
}
