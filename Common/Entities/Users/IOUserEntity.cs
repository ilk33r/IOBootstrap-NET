﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

	}
}
