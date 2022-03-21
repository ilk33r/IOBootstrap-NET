using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.Messages.MW
{
    public class IOMWUpdateTokenRequestModel : IORequestModel
    {
        public int ID { get; set; }
        public string UserToken { get; set; }
        public DateTimeOffset TokenDate { get; set; }
    }
}
