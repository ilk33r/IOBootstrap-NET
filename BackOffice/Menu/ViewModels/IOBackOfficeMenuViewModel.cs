using System;
using IOBootstrap.Net.Common.Messages.MW;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Models.Menu;
using IOBootstrap.NET.Core.ViewModels;

namespace IOBootstrap.NET.BackOffice.Menu.ViewModels
{
    public class IOBackOfficeMenuViewModel : IOBackOfficeViewModel
    {

        #region Initialization Methods

        public IOBackOfficeMenuViewModel() : base()
        {
        }

        #endregion

        #region Menu Methods

        //TODO: Migrate with MW.
        /*
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
        */
        public virtual IList<IOMenuListModel> GetMenuTree(int requiredRole)
        {
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeMenuControllerNameKey);
            IOMWFindRequestModel requestModel = new IOMWFindRequestModel()
            {
                ID = requiredRole
            };
            IOMWListResponseModel<IOMenuListModel> menuTreeResponse = MWConnector.Get<IOMWListResponseModel<IOMenuListModel>>(controller + "/" + "GetMenuTree", requestModel);
            return menuTreeResponse.Items;
        }
        /*
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
        */
        #endregion
    }
}
