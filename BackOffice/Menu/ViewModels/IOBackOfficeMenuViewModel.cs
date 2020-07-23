using System;
using System.Collections.Generic;
using System.Linq;
using IOBootstrap.NET.Common.Messages.Menu;
using IOBootstrap.NET.Common.Models.Menu;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.BackOffice.Menu.ViewModels
{
    public class IOBackOfficeMenuViewModel<TDBContext> : IOBackOfficeViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Initialization Methods

        public IOBackOfficeMenuViewModel() : base()
        {
        }

        #endregion

        #region Menu Methods

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
            var parentMenuTree = DatabaseContext.Menu.Where((arg) => arg.RequiredRole >= requiredRole && arg.ParentEntityID == null)
                                                     .OrderBy((arg) => arg.MenuOrder);

            if (parentMenuTree != null)
            {
                List<IOMenuListModel> menuTree = parentMenuTree.ToList()
                                                               .ConvertAll(parentMenuEntity =>
                                                               {
                                                                   IOMenuListModel menuListModel = new IOMenuListModel()
                                                                   {
                                                                       ID = parentMenuEntity.ID,
                                                                       MenuOrder = parentMenuEntity.MenuOrder,
                                                                       RequiredRole = parentMenuEntity.RequiredRole,
                                                                       Action = parentMenuEntity.Action,
                                                                       CssClass = parentMenuEntity.CssClass,
                                                                       Name = parentMenuEntity.Name,
                                                                   };

                                                                   return menuListModel;

                                                               });

                foreach (IOMenuListModel menuEntity in menuTree)
                {
                    var childMenuTree = DatabaseContext.Menu.Where((arg) => arg.RequiredRole >= requiredRole && arg.ParentEntityID == menuEntity.ID)
                                                            .OrderBy((arg) => arg.MenuOrder);
                    List<IOMenuListModel> childMenu = new List<IOMenuListModel>();

                    if (childMenuTree != null && childMenuTree.Count() > 0)
                    {
                        foreach (IOMenuEntity childMenuEntity in childMenuTree)
                        {
                            IOMenuListModel childMenuModel = new IOMenuListModel()
                            {
                                ID = childMenuEntity.ID,
                                Action = childMenuEntity.Action,
                                CssClass = childMenuEntity.CssClass,
                                Name = childMenuEntity.Name,
                                MenuOrder = childMenuEntity.MenuOrder,
                                RequiredRole = childMenuEntity.RequiredRole
                            };

                            childMenu.Add(childMenuModel);
                        }
                    }

                    menuEntity.ChildItems = childMenu;
                }

                return menuTree;
            }

            return new List<IOMenuListModel>();
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

        #endregion
    }
}
