using System;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.WebApi.Index.ViewModels
{
    public class IOIndexViewModel : IOViewModel<IODatabaseContextDefaultImpl>
    {        
        public override bool CheckAuthorizationHeader()
        {
            return true;
        }
    }
}
