using System;
using IOBootstrap.Net.Common.Messages.MW;
using IOBootstrap.NET.MW.Core.ViewModels;
using IOBootstrap.NET.MW.DataAccess.Context;
using IOBootstrap.NET.MW.DataAccess.Entities;

namespace IOBootstrap.NET.MW.WebApi.BackOffice.ViewModels
{
    public abstract class IOBackOfficeAuthenticationViewModel<TDBContext> : IOMWViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {

        public IOMWUserResponseModel FindUserFromName(string where)
        {
            return DatabaseContext.Users
                                    .Select(u => new IOMWUserResponseModel()
                                    {
                                        ID = u.ID,
                                        Password = u.Password,
                                        UserName = u.UserName,
                                        UserRole = u.UserRole,
                                        UserToken = u.UserToken,
                                        TokenDate = u.TokenDate
                                    })
                                    .Where(u => u.UserName.Equals(where))
                                    .FirstOrDefault();
        }

        public IOMWUserResponseModel FindUserById(int id)
        {
            return DatabaseContext.Users
                                    .Select(u => new IOMWUserResponseModel()
                                    {
                                        ID = u.ID,
                                        Password = u.Password,
                                        UserName = u.UserName,
                                        UserRole = u.UserRole,
                                        UserToken = u.UserToken,
                                        TokenDate = u.TokenDate
                                    })
                                    .Where(u => u.ID == id)
                                    .FirstOrDefault();
        }

        public void UpdateUserToken(int id, string newToken, DateTimeOffset tokenDate)
        {
            IOUserEntity user = DatabaseContext.Users.Find(id);
            user.UserToken = newToken;
            user.TokenDate = tokenDate;
            
            DatabaseContext.Update(user);
            DatabaseContext.SaveChanges();
        }
    }
}
