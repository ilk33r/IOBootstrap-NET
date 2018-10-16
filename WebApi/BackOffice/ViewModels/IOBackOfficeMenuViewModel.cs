﻿using System;
using System.Collections.Generic;
using System.Linq;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.WebApi.BackOffice.Entities;
using IOBootstrap.NET.WebApi.BackOffice.Models;

namespace IOBootstrap.NET.WebApi.BackOffice.ViewModels
{
    public class IOBackOfficeMenuViewModel<TDBContext> : IOBackOfficeViewModel<TDBContext>
        where TDBContext : IODatabaseContext<TDBContext>
    {
     
        #region Initialization Methods

        public IOBackOfficeMenuViewModel() : base()
        {
        }

        #endregion

        #region Menu Methods

        public IList<IOMenuListModel> GetMenuTree(int requiredRole)
        {
            var parentMenuTree = _databaseContext.Menu.Where((arg) => arg.RequiredRole >= requiredRole && arg.ParentEntityID == null)
                                                      .OrderBy((arg) => arg.MenuOrder);

            if (parentMenuTree != null)
            {
                List<IOMenuListModel> menuTree = new List<IOMenuListModel>();

                foreach (IOMenuEntity menuEntity in parentMenuTree)
                {
                    var childMenuTree = _databaseContext.Menu.Where((arg) => arg.RequiredRole >= requiredRole && arg.ParentEntityID == menuEntity.ID)
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
                                Name = childMenuEntity.Name
                            };

                            childMenu.Add(childMenuModel);
                        }
                    }

                    IOMenuListModel parentMenuModel = new IOMenuListModel()
                    {
                        ID = menuEntity.ID,
                        MenuOrder = menuEntity.MenuOrder,
                        RequiredRole = menuEntity.RequiredRole,
                        Action = menuEntity.Action,
                        CssClass = menuEntity.CssClass,
                        Name = menuEntity.Name,
                        ChildItems = childMenu
                    };

                    menuTree.Add(parentMenuModel);
                }

                return menuTree;
            }

            return new List<IOMenuListModel>();
        }

        #endregion
    }
}