using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IOBootstrap.NET.MW.DataAccess.Entities
{
    public class IOConfigurationEntity
    {

        #region Properties

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [StringLength(128)]
        public string ConfigKey { get; set; }

        public int? ConfigIntValue { get; set; }

        public string ConfigStringValue { get; set; }

        #endregion
    }
}
