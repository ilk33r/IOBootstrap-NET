using System;
using System.ComponentModel.DataAnnotations;

namespace IOBootstrap.NET.Common.Messages.Clients
{
    public class IOClientUpdateRequestModel: IOClientAddRequestModel
    {

        #region properties

        [Required]
        public int ClientId { get; set; }

        [Required]
        public int IsEnabled { get; set; }

        [Required]
        public long MaxRequestCount { get; set; }

        #endregion
    }
}
