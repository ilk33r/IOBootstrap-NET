using System;
using IOBootstrap.NET.Common.Messages.Base;

namespace IOBootstrap.NET.Common.MWConnector
{
    public interface IOMWConnectorProtocol
    {
        public delegate void IOMWConnectorResponseHandler(int code);
        public TObject Get<TObject>(string path, Object request) where TObject : IOResponseModel, new();
        public bool HandleResponse<TObject>(TObject response, IOMWConnectorResponseHandler handler) where TObject : IOResponseModel, new();
    }
}
