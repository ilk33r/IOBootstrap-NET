using IOBootstrap.NET.Common.Entities.AutoIncrements;
using IOBootstrap.NET.Common.Entities.Users;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.WebApi.BackOffice.ViewModels;
using IOBootstrap.NET.WebApi.User.Models;
using Realms;
using System;
using System.Linq;
using System.Collections.Generic;

namespace IOBootstrap.NET.WebApi.User.ViewModels
{
    public class IOUserViewModel : IOBackOfficeViewModel
    {

        #region Initialization Methods

        public IOUserViewModel() : base()
        {
        }

        #endregion

        #region View Model Methods

        public Tuple<bool, int, string> AddUser(string userName, string password, int userRole)
        {
			// Obtain realm instance
			Realm realm = this.Database.GetRealmForMainThread();

			// Obtain users entity
			var usersEntities = realm.All<IOUserEntity>()
												 .Where((arg) => arg.UserName == userName);

			// Check push notification entity exists
			if (usersEntities.Count() > 0)
			{
				// Dispose realm
				realm.Dispose();

                // Return user exists response
                return new Tuple<bool, int, string>(false, 0, null);
			}

			// Generate user id
			int userId = IOAutoIncrementsEntity.IdForClass(this.Database, typeof(IOUserEntity));

			// Create a users entity 
			IOUserEntity userEntity = new IOUserEntity()
			{
				ID = userId,
				UserName = userName.ToLower(),
				Password = IOCommonHelpers.HashPassword(password),
				UserRole = userRole,
				UserToken = null,
				TokenDate = DateTime.UtcNow
			};

			// Write user to database
			this.Database.InsertEntity(userEntity)
					 .Subscribe();

			// Dispose realm
			realm.Dispose();

            // Return status
            return new Tuple<bool, int, string>(true, userId, userName);
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword) 
        {
			// Obtain realm instance 
			Realm realm = this.Database.GetRealmForThread();

			// Obtain user entity
			var userEntities = realm.All<IOUserEntity>()
										   .Where((arg1) => arg1.UserName == userName.ToLower());

			// Check user finded
			if (userEntities.Count() > 0)
			{
				// Obtain user entity
				IOUserEntity user = userEntities.First();

				// Check user old password is valid
				if (IOCommonHelpers.VerifyPassword(oldPassword, user.Password))
				{
					// Begin write transaction
					Transaction realmTransaction = realm.BeginWrite();

					// Update user password properties
                    user.Password = IOCommonHelpers.HashPassword(newPassword);
					user.UserToken = null;

					// Update user password
					realm.Add(user, true);

					// Commit transaction
					realmTransaction.Commit();

					// Dispose realm
					realm.Dispose();

                    // Return response
                    return true;
				}
			}

			// Dispose realm
			realm.Dispose();

            // Return response
            return false;
        }

        public List<IOUserInfoModel> ListUsers()
        {
			// Create list for clients
			List<IOUserInfoModel> users = new List<IOUserInfoModel>();

			// Obtain realm 
			Realm realm = this.Database.GetRealmForMainThread();

			// Check real is not null
			if (realm != null)
			{
				// Obtain users from realm
				var user = realm.All<IOUserEntity>();

				// Check users is not null
				if (user != null)
				{
					// Loop throught clients
					for (int i = 0; i < user.Count(); i++)
					{
						// Obtain users entity
						IOUserEntity userEntity = user.ElementAt(i);

						// Create user info model
						IOUserInfoModel model = new IOUserInfoModel()
						{
							ID = userEntity.ID,
							UserName = userEntity.UserName,
							UserRole = userEntity.UserRole,
							UserToken = userEntity.UserToken,
							TokenDate = userEntity.TokenDate
						};

						// Add model to user list
						users.Add(model);
					}
				}
			}

			// Dispose realm
			realm.Dispose();

            // Return users
            return users;
        }

        #endregion
    }
}
