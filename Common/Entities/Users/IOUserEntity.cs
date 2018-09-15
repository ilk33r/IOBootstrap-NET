using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IOBootstrap.NET.Common.Entities.Users
{
    public class IOUserEntity
    {

        #region Properties

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [StringLength(255)]
        public string UserName { get; set; }

        public string Password { get; set; }
        public int UserRole { get; set; }
        public string UserToken { get; set; }
        public DateTimeOffset TokenDate { get; set; }

        #endregion

        #region Helper Methods

        public static IOUserEntity FindUserFromName(DbSet<IOUserEntity> users, string userName)
        {
            // Obtain user entity
            var userEntities = users.Where((arg1) => arg1.UserName == userName.ToLower());

            // Check user finded
            if (userEntities.Count() > 0)
            {
                // Obtain user entity
                return userEntities.First();
            }

            return null;
        }

        #endregion
    }
}
