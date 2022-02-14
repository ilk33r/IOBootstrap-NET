using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.Net.Common.MWConnector
{
    public interface IOMWConnectorProtocol
    {
        public TObject Get<TObject>(string path, Object request) where TObject : IOResponseModel, new();
    }
}
