using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IOBootstrap.NET.Common.Entities.Clients
{
    public class IOClientsEntity
    {

		#region Properties

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

		[StringLength(32)]
        public string ClientId { get; set; }

		[StringLength(64)]
        public string ClientSecret { get; set; }
        public string ClientDescription { get; set; }

        #endregion

    }
}
