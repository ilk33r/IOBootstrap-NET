using System;
using IOBootstrap.NET.Core.ViewModels;

namespace IOBootstrap.NET.WebApi.KeyGenerator.ViewModels
{
    public class IOKeyGeneratorViewModel : IOViewModel
    {

        public override void CheckAuthorizationHeader()
        {
            #if DEBUG
            return;
            #else
            base.CheckAuthorizationHeader();
            #endif
        }
    }
}
