import UserInfoModel from "../models/UserInfoModel";

class UsersListState {

    userList: UserInfoModel[];

    constructor() {
        this.userList = [];
    }
}

export default UsersListState;
