import MenuDeleteRequestModel from "../models/MenuDeleteRequestModel";
import MenuEditorListProps from "../props/MenuEditorListProps";
import MenuEditorListState from "../props/MenuEditorListState";
import MenuListModel from "../models/MenuListModel";
import MenuListResponseModel from "../models/MenuListResponseModel";
import MenuUpdateRequestModel from "../models/MenuUpdateRequestModel";
import React from "react";
import { DIHooks } from "iobootstrap-ui-base";
import { BOController, BreadcrumbNavigationModel, ListDataItemModel, ListView } from "iobootstrap-bo-base";

class MenuEditorListController extends BOController<MenuEditorListProps, MenuEditorListState> {

    private _menuItems: MenuListModel[];

    constructor(props: MenuEditorListProps) {
        super(props);

        this._menuItems = [];

        this.state = new MenuEditorListState();

        this.deleteDataHandler = this.deleteDataHandler.bind(this);
        this.updateDataHandler = this.updateDataHandler.bind(this);
    }

    public componentDidMount?(): void {
        this.appContext.removeObject("menuEditorDeleteRequest");
        this.appContext.removeObject("menuEditorUpdateRequest");

        this.indicatorPresenter.present();

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_MENU_CONTROLLER_NAME}/ListMenuItems`;
        const weakSelf = this;

        this.service.get(requestPath, function (response: MenuListResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                const newState = new MenuEditorListState();
                newState.menuList = response.items;

                weakSelf.setState(newState);
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    deleteDataHandler(index: number) {
        const selectedMenu = this._menuItems[index];
        const deleteRequestModel = new MenuDeleteRequestModel();
        deleteRequestModel.id = selectedMenu.id;
        
        this.appContext.setObjectForKey("menuEditorDeleteRequest", deleteRequestModel);
        this.navigateToPage("menuEditorDelete");
    }

    updateDataHandler(index: number) {
        let currentIndex = 0;
        let itemFound = false;

        for (const parentMenu of this.state.menuList) {
            if (currentIndex === index) {
                const updateRequestModel = new MenuUpdateRequestModel();
                updateRequestModel.id = parentMenu.id;
                updateRequestModel.action = parentMenu.action;
                updateRequestModel.cssClass = parentMenu.cssClass;
                updateRequestModel.name = parentMenu.name;
                updateRequestModel.menuOrder = parentMenu.menuOrder;
                updateRequestModel.requiredRole = parentMenu.requiredRole;
                updateRequestModel.parentEntityID = null;
                updateRequestModel.parentEntityName = null;

                this.appContext.setObjectForKey("menuEditorUpdateRequest", updateRequestModel);
                this.navigateToPage("menuEditorUpdate");
                break;
            }

            currentIndex += 1;

            if (parentMenu.childItems.length > 0) {
                for (let childMenu of parentMenu.childItems) {
                    if (currentIndex === index) {
                        const updateRequestModel = new MenuUpdateRequestModel();
                        updateRequestModel.id = childMenu.id;
                        updateRequestModel.action = childMenu.action;
                        updateRequestModel.cssClass = childMenu.cssClass;
                        updateRequestModel.name = childMenu.name;
                        updateRequestModel.menuOrder = childMenu.menuOrder;
                        updateRequestModel.requiredRole = childMenu.requiredRole;
                        updateRequestModel.parentEntityID = parentMenu.id;
                        updateRequestModel.parentEntityName = parentMenu.name;
    
                        itemFound = true;
                        this.appContext.setObjectForKey("menuEditorUpdateRequest", updateRequestModel);
                        this.navigateToPage("menuEditorUpdate");
                        break;
                    }

                    currentIndex += 1;
                }

                if (itemFound) {
                    break;
                }
            }
        }
    }

    render() {
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("menuEditorList", "Menu Editor")
        ];

        const listDataHeaders = [
            'ID',
            'Name',
            'Action',
            'Css Class',
            'Role',
            'Order'
        ];

        let items: ListDataItemModel[] = [];

        this.state.menuList.forEach(menu => {
            const itemModel = new ListDataItemModel();

            let roleName = ""
            const userRoleNameHook = DIHooks.Instance.hookForKey("userRoleName")
            if (userRoleNameHook != null) {
                const roleNameAny = userRoleNameHook(menu.requiredRole);
                if (roleNameAny != null) {
                    roleName = roleNameAny;
                }
            }

            itemModel.itemList = [
                menu.id.toString(),
                menu.name.RemoveHTML(),
                menu.action.RemoveHTML(),
                menu.cssClass.RemoveHTML(),
                roleName,
                `<strong>${menu.menuOrder.toString()}</strong>`
            ];

            items.push(itemModel);
            this._menuItems.push(menu);

            if (menu.childItems.length > 0) {
                menu.childItems.forEach(childMenu => {
                    const childItemModel = new ListDataItemModel();

                    let childMenuRoleName = "";
                    if (userRoleNameHook != null) {
                        const childMenuRoleNameAny = userRoleNameHook(childMenu.requiredRole);
                        if (childMenuRoleNameAny != null) {
                            childMenuRoleName = childMenuRoleNameAny;
                        }
                    }
        
                    childItemModel.itemList = [
                        childMenu.id.toString(),
                        childMenu.name.RemoveHTML(),
                        childMenu.action.RemoveHTML(),
                        childMenu.cssClass.RemoveHTML(),
                        childMenuRoleName,
                        `<strong>${childMenu.menuOrder.toString()}</strong>`
                    ];
        
                    childItemModel.isEven = true;
                    items.push(childItemModel);
                    this._menuItems.push(childMenu);
                });
            }
        });

        return (
            <React.StrictMode>
                <ListView navigation={navigation} 
                    listDataHeaders={listDataHeaders} 
                    items={items}
                    resourceDelete="Delete"
                    resourceEdit="Edit"
                    resourceHome="Home"
                    resourceOptions="Options"
                    resourceSelect=""
                    extras={null}
                    deleteDataHandler={this.deleteDataHandler}
                    updateDataHandler={this.updateDataHandler}
                    selectDataHandler={null}
                    itemVisibleHandler={null}
                    pagination={null} />
            </React.StrictMode>
        );
    }
}

export default MenuEditorListController;
