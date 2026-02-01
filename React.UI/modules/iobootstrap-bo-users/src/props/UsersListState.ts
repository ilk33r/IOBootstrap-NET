import UserInfoModel from "../models/UserInfoModel";

class UsersListState {

    count: number;
    userList: UserInfoModel[];

    constructor() {
        this.count = 0;
        this.userList = [];
    }
}

export default UsersListState;
