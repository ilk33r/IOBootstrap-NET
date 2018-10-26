using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IOBootstrap.NET.WebApi.BackOffice.Entities
{
    public class IOBackOfficeMessageEntity
    {
        /*
         * We've got something special for you. 
         * On 26 October, there will be no access due to maintenance work at 15:00 PM (GMT) - 16:00 PM (GMT).
         */

        #region Properties

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string Message { get; set; }

        public DateTimeOffset MessageCreateDate { get; set; }
        public DateTimeOffset MessageStartDate { get; set; }
        public DateTimeOffset MessageEndDate { get; set; }

        #endregion
    }
}
