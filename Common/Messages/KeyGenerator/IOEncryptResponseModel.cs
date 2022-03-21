using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.KeyGenerator
{
    public class IOEncryptResponseModel : IOResponseModel
    {
        #region Properties

        public string SymmetricKey { get; set; }
        public string SymmetricIV { get; set; }
        public string EncryptedValue { get; set; }

		#endregion
    }
}
