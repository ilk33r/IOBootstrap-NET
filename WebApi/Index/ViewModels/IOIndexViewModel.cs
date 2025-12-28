using System;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.WebApi.Index.ViewModels;

public class IOIndexViewModel<TDBContext> : IOViewModel<TDBContext>
where TDBContext : IOBaseDatabaseContext<TDBContext>
{
    public override void CheckAuthorizationHeader()
    {
        return;
    }
}
