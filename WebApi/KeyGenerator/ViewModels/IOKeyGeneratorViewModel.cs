using System;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.WebApi.KeyGenerator.ViewModels
{
    public class IOKeyGeneratorViewModel : IOViewModel<IODatabaseContextDefaultImpl>
    {

        public override bool CheckAuthorizationHeader()
        {
            #if DEBUG
            return true;
            #else
            return base.CheckAuthorizationHeader();
            #endif
        }
    }
}
