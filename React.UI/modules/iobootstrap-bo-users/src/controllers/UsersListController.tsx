import DeleteUserRequestModel from "../models/DeleteUserRequestModel";
import ListUserResponseModel from "../models/ListUserResponseModel";
import React from "react";
import UpdateUserRequestModel from "../models/UpdateUserRequestModel";
import UsersListProps from "../props/UsersListProps";
import UsersListState from "../props/UsersListState";
import { Controller, DIHooks } from "iobootstrap-ui-base";
import { BreadcrumbNavigationModel, ListDataItemModel, ListExtrasModel, ListView } from "iobootstrap-bo-base";

class UsersListController extends Controller<UsersListProps, UsersListState> {

    constructor(props: UsersListProps) {
        super(props);

        this.state = new UsersListState();

        this.changePasswordHandler = this.changePasswordHandler.bind(this);
        this.deleteDataHandler = this.deleteDataHandler.bind(this);
        this.updateDataHandler = this.updateDataHandler.bind(this);
    }

    public componentDidMount?(): void {
        this.appContext.removeObject("usersChangePasswordRequest");
        this.appContext.removeObject("usersDeleteRequest");
        this.appContext.removeObject("usersUpdateRequest");

        this.indicatorPresenter.present();

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_USER_CONTROLLER_NAME}/ListUsers`;
        const weakSelf = this;

        this.service.get(requestPath, function (response: ListUserResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                const newState = new UsersListState();
                newState.userList = response.users;

                weakSelf.setState(newState);
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    changePasswordHandler(index: number) {
        const currentUser = this.state.userList[index];
        const updateRequestModel = new UpdateUserRequestModel();
        updateRequestModel.userId = currentUser.id;
        updateRequestModel.userName = currentUser.userName;
        updateRequestModel.userRole = currentUser.userRole;

        this.appContext.setObjectForKey("usersChangePasswordRequest", updateRequestModel);
        this.navigateToPage("userChangePassword");
    }

    deleteDataHandler(index: number) {
        const currentUser = this.state.userList[index];
        const deleteRequestModel = new DeleteUserRequestModel();
        deleteRequestModel.userId = currentUser.id;
        
        this.appContext.setObjectForKey("usersDeleteRequest", deleteRequestModel);
        this.navigateToPage("usersDelete");
    }

    updateDataHandler(index: number) {
        const currentUser = this.state.userList[index];
        const updateRequestModel = new UpdateUserRequestModel();
        updateRequestModel.userId = currentUser.id;
        updateRequestModel.userName = currentUser.userName;
        updateRequestModel.userRole = currentUser.userRole;

        this.appContext.setObjectForKey("usersUpdateRequest", updateRequestModel);
        this.navigateToPage("usersUpdate");
    }

    render() {
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("usersList", "Users")
        ];

        const listDataHeaders = [
            'ID',
            'Name',
            'Role',
            'Last Login Date'
        ];

        const items = this.state.userList.map(user => {
            const itemModel = new ListDataItemModel();

            let roleName = ""
            const userRoleNameHook = DIHooks.Instance.hookForKey("userRoleName")
            if (userRoleNameHook != null) {
                const roleNameAny = userRoleNameHook(user.userRole);
                if (roleNameAny != null) {
                    roleName = roleNameAny;
                }
            }

            itemModel.itemList = [
                user.id.toString(),
                user.userName,
                roleName,
                user.tokenDate ?? ""
            ];

            return itemModel;
        });

        const extras = [
            new ListExtrasModel("Change Password", "fa-key", this.changePasswordHandler)
        ];

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
                    extras={extras}
                    deleteDataHandler={this.deleteDataHandler}
                    updateDataHandler={this.updateDataHandler}
                    selectDataHandler={null}
                    pagination={null} />
            </React.StrictMode>
        );
    }
}

export default UsersListController;
