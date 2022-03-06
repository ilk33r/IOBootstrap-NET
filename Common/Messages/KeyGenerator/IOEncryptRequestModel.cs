using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.Net.Common.Messages.KeyGenerator
{
    public class IOEncryptRequestModel : IORequestModel
    {
        public string PublicKeyExponent { get; set; }
        public string PublicKeyModulus { get; set; }
        public string PlainText { get; set; }
    }
}
