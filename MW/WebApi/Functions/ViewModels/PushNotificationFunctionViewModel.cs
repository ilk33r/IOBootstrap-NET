using System;
using IOBootstrap.NET.MW.Core.ViewModels;
using IOBootstrap.NET.MW.DataAccess.Context;

namespace IOBootstrap.NET.MW.WebApi.Functions.ViewModels
{
    public class PushNotificationFunctionViewModel<TDBContext> : IOMWViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {
    }
}
