import DeleteUserRequestModel from "../models/DeleteUserRequestModel";
import ListUserResponseModel from "../models/ListUserResponseModel";
import UpdateUserRequestModel from "../models/UpdateUserRequestModel";
import UsersListProps from "../props/UsersListProps";
import UsersListState from "../props/UsersListState";
import { BaseView, DIHooks } from "iobootstrap-ui-base";
import { BOCommonConstants, BOController, BreadcrumbNavigationModel, ListDataFilterTypes, ListDataHeaderModel, ListDataItemModel, ListDataPaginationModel, ListExtrasModel, ListView, UserRoles } from "iobootstrap-bo-base";
import ListUserRequestModel from "../models/ListUserRequestModel";

class UsersListController extends BOController<UsersListProps, UsersListState> {

    private requestModel: ListUserRequestModel;

    constructor(props: UsersListProps) {
        super(props);

        this.requestModel = new ListUserRequestModel();

        this.state = new UsersListState();

        this.changePasswordHandler = this.changePasswordHandler.bind(this);
        this.deleteDataHandler = this.deleteDataHandler.bind(this);
        this.updateDataHandler = this.updateDataHandler.bind(this);
        this.itemVisibleHandler = this.itemVisibleHandler.bind(this);
        this.pageChangeHandler = this.pageChangeHandler.bind(this);
    }

    public componentDidMount?(): void {
        this.appContext.removeObject("usersChangePasswordRequest");
        this.appContext.removeObject("usersDeleteRequest");
        this.appContext.removeObject("usersUpdateRequest");

        this.requestModel.start = 0;
        this.requestModel.count = 25;

        this.loadUsers();
    }

    private loadUsers() {
        this.indicatorPresenter.present();

        const requestPath = `${import.meta.env.VITE_BACKOFFICE_USER_CONTROLLER_NAME}/ListUsers`;
        const weakSelf = this;

        this.service.post(requestPath, this.requestModel, function (response: ListUserResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                const newState = new UsersListState();
                newState.userList = response.users;
                newState.count = response.count ?? 0;

                weakSelf.setState(newState);
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    private pageChangeHandler(start: number, length: number) {
        this.requestModel.start = start;
        this.requestModel.count = length;
        this.loadUsers();
    }

    private changePasswordHandler(index: number) {
        const currentUser = this.state.userList[index];

        if (currentUser.userName == this.appContext.stringForKey(BOCommonConstants.userNameStorageKey)) {
            this.navigateToPage("userChangePassword");
            return;
        }

        const updateRequestModel = new UpdateUserRequestModel();
        updateRequestModel.userId = currentUser.id;
        updateRequestModel.userName = currentUser.userName;
        updateRequestModel.userRole = currentUser.userRole;

        this.appContext.setObjectForKey("usersResetPasswordRequest", updateRequestModel);
        this.navigateToPage("userResetPassword");
    }

    private deleteDataHandler(index: number) {
        const currentUser = this.state.userList[index];
        const deleteRequestModel = new DeleteUserRequestModel();
        deleteRequestModel.userId = currentUser.id;
        
        this.appContext.setObjectForKey("usersDeleteRequest", deleteRequestModel);
        this.navigateToPage("usersDelete");
    }

    private updateDataHandler(index: number) {
        const currentUser = this.state.userList[index];
        const updateRequestModel = new UpdateUserRequestModel();
        updateRequestModel.userId = currentUser.id;
        updateRequestModel.userName = currentUser.userName;
        updateRequestModel.userRole = currentUser.userRole;
        updateRequestModel.isActive = currentUser.isActive;
        updateRequestModel.activationEndDate = currentUser.activationEndDate;

        this.appContext.setObjectForKey("usersUpdateRequest", updateRequestModel);
        this.navigateToPage("usersUpdate");
    }

    private itemVisibleHandler(listIndex: number, itemIndex: number): boolean {
        const currentUserRole = this.appContext.numberForKey(BOCommonConstants.userRoleStorageKey) ?? UserRoles.AnonmyMouse;
        const currentUserName = this.appContext.stringForKey(BOCommonConstants.userNameStorageKey) ?? "";
        const listUser = this.state.userList[listIndex];

        if ((itemIndex == 1 || itemIndex == 3) && listUser.userName == currentUserName) {
            return false;
        }

        if (currentUserRole == UserRoles.SuperAdmin) {
            return true;
        }

        if (currentUserRole < listUser.userRole) {
            return true;
        }
        
        return false;
    }

    render() {
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("usersList", "Users")
        ];

        const roleNames = this.state.userList.map(user => { 
            let roleName = ""
            const userRoleNameHook = DIHooks.Instance.hookForKey("userRoleName")
            if (userRoleNameHook != null) {
                const roleNameAny = userRoleNameHook(user.userRole);
                if (roleNameAny != null) {
                    roleName = roleNameAny;
                }
            }

            return roleName
        });

        const headers = [
            ListDataHeaderModel.initialize("ID"),
            ListDataHeaderModel.initializeWithFilter("Name", ListDataFilterTypes.Input, null),
            ListDataHeaderModel.initializeWithFilter("Role", ListDataFilterTypes.Select, roleNames),
            ListDataHeaderModel.initializeWithFilter("Active", ListDataFilterTypes.Select, ["YES", "NO"]),
            ListDataHeaderModel.initialize("End Date"),
            ListDataHeaderModel.initialize("Created Date"),
            ListDataHeaderModel.initialize("Created By"),
            ListDataHeaderModel.initialize("Update Date"),
            ListDataHeaderModel.initialize("Last Login Date"),
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

            const tokenDate = (user.tokenDate === undefined || user.tokenDate === null) ? "-" : new Date(user.tokenDate).toLocaleDateString('en-US', { year: 'numeric', day: '2-digit', month: '2-digit' });
            const userIsActive = (user.isActive) ? "<strong><span class=\"text-success\">YES</span></strong>" : "<strong><span class=\"text-danger\">NO</span></strong>";
            const activationEndDate = (user.activationEndDate === undefined || user.activationEndDate === null) ? "-" : new Date(user.activationEndDate).toLocaleDateString('en-US', { year: 'numeric', day: '2-digit', month: '2-digit' });
            const createdDate = (user.createdDate === undefined || user.createdDate === null) ? "-" : new Date(user.createdDate).toLocaleDateString('en-US', { year: 'numeric', day: '2-digit', month: '2-digit' });
            const updateDate = (user.updateDate === undefined || user.updateDate === null) ? "-" : new Date(user.updateDate).toLocaleDateString('en-US', { year: 'numeric', day: '2-digit', month: '2-digit' });

            itemModel.itemList = [
                user.id.toString(),
                `<em>${user.userName.RemoveHTML()}</em>`,
                `<strong>${roleName}</strong>`,
                userIsActive,
                `<u>${activationEndDate}</u>`,
                createdDate,
                `<em>${(user.createdBy ?? "-").RemoveHTML()}</em>`,
                updateDate,
                tokenDate,
            ];

            itemModel.textWraps = [
                false,
                false,
                false,
                true,
                true,
                false,
                true,
                true,
                false
            ];

            return itemModel;
        });

        const extras = [
            new ListExtrasModel("Reset Password", "fa-key", this.changePasswordHandler)
        ];

        const pagination = new ListDataPaginationModel();
        pagination.start = this.requestModel.start;
        pagination.length = this.requestModel.count;
        pagination.count = this.state.count;
        pagination.pageClickHandler = this.pageChangeHandler;

        return (
            <BaseView>
                <ListView navigation={navigation} 
                    headers={headers} 
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
                    itemVisibleHandler={this.itemVisibleHandler}
                    pagination={pagination} />
            </BaseView>
        );
    }
}

export default UsersListController;
