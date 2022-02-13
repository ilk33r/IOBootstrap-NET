using System;
using IOBootstrap.NET.MW.Core.ViewModels;
using IOBootstrap.NET.MW.DataAccess.Context;

namespace IOBootstrap.NET.MW.WebApi.Index.ViewModels
{
    public class IOMWIndexViewModel<TDBContext> : IOMWViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {
    }
}
