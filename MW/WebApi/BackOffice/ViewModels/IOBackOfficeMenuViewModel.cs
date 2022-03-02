using System;
using IOBootstrap.NET.Common.Messages.Menu;
using IOBootstrap.NET.Common.Models.Menu;
using IOBootstrap.NET.MW.Core.ViewModels;
using IOBootstrap.NET.MW.DataAccess.Context;
using IOBootstrap.NET.MW.DataAccess.Entities;

namespace IOBootstrap.NET.MW.WebApi.BackOffice.ViewModels
{
    public abstract class IOBackOfficeMenuViewModel<TDBContext> : IOMWViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {
        public void AddMenuItem(IOMenuAddRequestModel requestModel)
        {
            // Create menu item entity
            IOMenuEntity menuEntity = new IOMenuEntity()
            {
                Action = requestModel.Action,
                CssClass = requestModel.CssClass,
                Name = requestModel.Name,
                MenuOrder = requestModel.MenuOrder,
                RequiredRole = requestModel.RequiredRole,
                ParentEntityID = null
            };

            // Check parent entity defined
            if (requestModel.ParentEntityID != null && requestModel.ParentEntityID != 0)
            {
                menuEntity.ParentEntityID = requestModel.ParentEntityID;
            }

            // Add menu entity to database
            DatabaseContext.Add(menuEntity);
            DatabaseContext.SaveChanges();
        }

        public void DeleteMenuItem(int menuId)
        {
            // Obtain menu item entity
            IOMenuEntity menuEntity = DatabaseContext.Find<IOMenuEntity>(menuId);

            // Check menu is not exists
            if (menuEntity == null)
            {
                return;
            }

            // Add menu entity to database
            DatabaseContext.Remove(menuEntity);
            DatabaseContext.SaveChanges();
        }

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

        public void UpdateMenuItem(IOMenuUpdateRequestModel requestModel)
        {
            // Obtain menu item entity
            IOMenuEntity menuEntity = DatabaseContext.Find<IOMenuEntity>(requestModel.ID);

            // Check menu is not exists
            if (menuEntity == null)
            {
                return;
            }

            // Update menu item entity
            menuEntity.Action = requestModel.Action;
            menuEntity.CssClass = requestModel.CssClass;
            menuEntity.Name = requestModel.Name;
            menuEntity.MenuOrder = requestModel.MenuOrder;
            menuEntity.RequiredRole = requestModel.RequiredRole;
            menuEntity.ParentEntityID = null;

            // Check parent entity defined
            if (requestModel.ParentEntityID != null && requestModel.ParentEntityID != 0)
            {
                menuEntity.ParentEntityID = requestModel.ParentEntityID;
            }

            // Add menu entity to database
            DatabaseContext.Update(menuEntity);
            DatabaseContext.SaveChanges();
        }
    }
}
