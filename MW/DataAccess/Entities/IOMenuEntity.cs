using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IOBootstrap.NET.MW.DataAccess.Entities
{
    public class IOMenuEntity
    {

        #region Properties

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [StringLength(255)]
        public string Action { get; set; }

        [StringLength(255)]
        public string CssClass { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public int MenuOrder { get; set; }
        
        public int RequiredRole { get; set; }

        public Nullable<int> ParentEntityID { get; set; }

        #endregion

    }
}
