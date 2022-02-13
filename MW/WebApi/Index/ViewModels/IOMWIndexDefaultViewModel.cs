using System;
using IOBootstrap.NET.MW.DataAccess.Context;

namespace IOBootstrap.NET.MW.WebApi.Index.ViewModels
{
    public class IOMWIndexDefaultViewModel : IOMWIndexViewModel<IODatabaseContextDefaultImpl>
    {

        public override void CheckAuthorizationHeader() 
        {
        }
    }
}
