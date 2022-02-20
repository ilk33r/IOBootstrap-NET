using System;
using IOBootstrap.NET.Common.Models.Menu;
using IOBootstrap.NET.MW.Core.ViewModels;
using IOBootstrap.NET.MW.DataAccess.Context;

namespace IOBootstrap.NET.MW.WebApi.BackOffice.ViewModels
{
    public abstract class IOBackOfficeMenuViewModel<TDBContext> : IOMWViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {
        public virtual IList<IOMenuListModel> GetMenuTree(int requiredRole)
        {
            List<IOMenuListModel> menuTree = DatabaseContext.Menu
                                                                .Select(m => new IOMenuListModel()
                                                                {
                                                                    ID = m.ID,
                                                                    MenuOrder = m.MenuOrder,
                                                                    RequiredRole = m.RequiredRole,
                                                                    Action = m.Action,
                                                                    CssClass = m.CssClass,
                                                                    Name = m.Name,
                                                                    ParentEntityID = m.ParentEntityID,
                                                                    ChildItems = DatabaseContext.Menu
                                                                                                    .Select(cm => new IOMenuListModel()
                                                                                                    {
                                                                                                        ID = cm.ID,
                                                                                                        MenuOrder = cm.MenuOrder,
                                                                                                        RequiredRole = cm.RequiredRole,
                                                                                                        Action = cm.Action,
                                                                                                        CssClass = cm.CssClass,
                                                                                                        Name = cm.Name,
                                                                                                        ParentEntityID = cm.ParentEntityID,
                                                                                                    })
                                                                                                    .Where(cm => cm.RequiredRole >= requiredRole && cm.ParentEntityID == m.ID)
                                                                                                    .OrderBy(cm => cm.MenuOrder)
                                                                                                    .ToList()
                                                                })
                                                                .Where(m => m.RequiredRole >= requiredRole && m.ParentEntityID == null)
                                                                .OrderBy(m => m.MenuOrder)
                                                                .ToList();

            return menuTree;
        }
    }
}
