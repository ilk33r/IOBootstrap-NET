using System;
using System.ComponentModel.DataAnnotations;

namespace IOBootstrap.NET.Common.Messages.Clients
{
    public class IOClientDeleteRequestModel
    {

        #region Properties

        [Required]
        public int ClientId { get; set; }

        #endregion

    }
}
