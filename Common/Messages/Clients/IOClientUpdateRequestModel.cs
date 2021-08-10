using System;
using System.ComponentModel.DataAnnotations;
using IOBootstrap.NET.Common.Messages.Base;

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
