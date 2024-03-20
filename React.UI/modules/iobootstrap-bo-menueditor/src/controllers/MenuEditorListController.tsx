import MenuDeleteRequestModel from "../models/MenuDeleteRequestModel";
import MenuEditorListProps from "../props/MenuEditorListProps";
import MenuEditorListState from "../props/MenuEditorListState";
import MenuListModel from "../models/MenuListModel";
import MenuListResponseModel from "../models/MenuListResponseModel";
import MenuUpdateRequestModel from "../models/MenuUpdateRequestModel";
import React from "react";
import { Controller } from "iobootstrap-ui-base";
import { BreadcrumbNavigationModel, ListDataItemModel, ListView, UserRoles } from "iobootstrap-bo-base";

class MenuEditorListController extends Controller<MenuEditorListProps, MenuEditorListState> {

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
    
                        this.appContext.setObjectForKey("menuEditorUpdateRequest", updateRequestModel);
                        this.navigateToPage("menuEditorUpdate");
                        break;
                    }

                    currentIndex += 1;
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

            const role: UserRoles = UserRoles.fromRawValue(menu.requiredRole);
            const roleName = UserRoles.getRoleName(role);

            itemModel.itemList = [
                menu.id.toString(),
                menu.name,
                menu.action,
                menu.cssClass,
                roleName,
                menu.menuOrder.toString()
            ];

            items.push(itemModel);
            this._menuItems.push(menu);

            if (menu.childItems.length > 0) {
                menu.childItems.forEach(childMenu => {
                    const childItemModel = new ListDataItemModel();

                    const childMenuRole: UserRoles = UserRoles.fromRawValue(childMenu.requiredRole);
                    const childMenuRoleName = UserRoles.getRoleName(childMenuRole);
        
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
                    pagination={null} />
            </React.StrictMode>
        );
    }
}

export default MenuEditorListController;
