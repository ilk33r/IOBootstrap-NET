using System;

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
