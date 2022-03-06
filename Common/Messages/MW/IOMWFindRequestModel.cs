using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.Net.Common.Messages.MW
{
    public class IOMWFindRequestModel : IORequestModel
    {

        public int ID { get; set; }
        public string Where { get; set; }

        public IOMWFindRequestModel() : base()
        {
        }
    }
}