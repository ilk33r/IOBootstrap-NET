using System;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.WebApi.Index.ViewModels
{
    public class IOIndexViewModel<TDBContext> : IOViewModel<TDBContext>
    where TDBContext : IODatabaseContext<TDBContext> 
    {   

        public override void CheckClient()
        {
            return;
        }

        public override void CheckAuthorizationHeader()
        {
            return;
        }
    }
}
