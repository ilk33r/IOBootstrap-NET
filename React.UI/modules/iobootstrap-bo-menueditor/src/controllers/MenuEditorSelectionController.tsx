import MenuEditorListProps from "../props/MenuEditorListProps";
import MenuEditorListState from "../props/MenuEditorListState";
import MenuListResponseModel from "../models/MenuListResponseModel";
import React from "react";
import { DIHooks, WindowMessageModel } from "iobootstrap-ui-base";
import { BOController, BreadcrumbNavigationModel, ListDataFilterTypes, ListDataHeaderModel, ListDataItemModel, ListView } from "iobootstrap-bo-base";

class MenuEditorSelectionController extends BOController<MenuEditorListProps, MenuEditorListState> {

    private _menuItems: WindowMessageModel[];

    constructor(props: MenuEditorListProps) {
        super(props);

        this._menuItems = [];

        this.state = new MenuEditorListState();

        this.selectDataHandler = this.selectDataHandler.bind(this);
    }

    public componentDidMount?(): void {
        this.appContext.removeObject("configurationDeleteRequest");
        this.appContext.removeObject("configurationUpdateRequest");

        this.indicatorPresenter.present();

        const requestPath = `${import.meta.env.VITE_BACKOFFICE_MENU_CONTROLLER_NAME}/ListMenuItems`;
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

    selectDataHandler(index: number) {
        const selectedMenu = this._menuItems[index];
        this.postMessage(selectedMenu.name, selectedMenu.itemID, selectedMenu.itemValue);
    }

    render() {
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("menuEditorList", "Select Menu")
        ];

        const headers = [
            ListDataHeaderModel.initialize("ID"),
            ListDataHeaderModel.initializeWithFilter("Name", ListDataFilterTypes.Input, null),
            ListDataHeaderModel.initializeWithFilter("Action", ListDataFilterTypes.Input, null),
            ListDataHeaderModel.initialize("Css Class"),
            ListDataHeaderModel.initializeWithFilter("Role", ListDataFilterTypes.Input, null),
            ListDataHeaderModel.initialize("Order"),
        ];

        const items: ListDataItemModel[] = [];
        this._menuItems = [];

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
                `<strong>${roleName}</strong>`,
                menu.menuOrder.toString()
            ];

            items.push(itemModel);
            this._menuItems.push({name: "itemSelected", itemID: menu.id, itemValue: menu.name});

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
                        `<strong>${childMenuRoleName}</strong>`,
                        childMenu.menuOrder.toString()
                    ];
        
                    childItemModel.isChild = true;
                    items.push(childItemModel);
                    this._menuItems.push({name: "itemSelected", itemID: childMenu.id, itemValue: childMenu.name});
                });
            }
        });

        return (
            <React.StrictMode>
                <ListView navigation={navigation} 
                    headers={headers} 
                    items={items}
                    resourceDelete=""
                    resourceEdit=""
                    resourceHome="Home"
                    resourceOptions="Options"
                    resourceSelect="Select"
                    extras={null}
                    deleteDataHandler={null}
                    updateDataHandler={null}
                    selectDataHandler={this.selectDataHandler}
                    itemVisibleHandler={null}
                    pagination={null} />
            </React.StrictMode>
        );
    }
}

export default MenuEditorSelectionController;
