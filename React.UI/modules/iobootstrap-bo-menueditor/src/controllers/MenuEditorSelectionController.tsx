import MenuEditorListProps from "../props/MenuEditorListProps";
import MenuEditorListState from "../props/MenuEditorListState";
import MenuListResponseModel from "../models/MenuListResponseModel";
import React from "react";
import { Controller, DIHooks, WindowMessageModel } from "iobootstrap-ui-base";
import { BreadcrumbNavigationModel, ListDataItemModel, ListView } from "iobootstrap-bo-base";

class MenuEditorSelectionController extends Controller<MenuEditorListProps, MenuEditorListState> {

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

    selectDataHandler(index: number) {
        const selectedMenu = this._menuItems[index];
        this.postMessage(selectedMenu);
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
                menu.name,
                menu.action,
                menu.cssClass,
                roleName,
                menu.menuOrder.toString()
            ];

            items.push(itemModel);
            this._menuItems.push({itemID: menu.id, itemValue: menu.name});

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
                        childMenu.name,
                        childMenu.action,
                        childMenu.cssClass,
                        childMenuRoleName,
                        childMenu.menuOrder.toString()
                    ];
        
                    childItemModel.isEven = true;
                    items.push(childItemModel);
                    this._menuItems.push({itemID: childMenu.id, itemValue: childMenu.name});
                });
            }
        });

        return (
            <React.StrictMode>
                <ListView navigation={navigation} 
                    listDataHeaders={listDataHeaders} 
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
                    pagination={null} />
            </React.StrictMode>
        );
    }
}

export default MenuEditorSelectionController;
