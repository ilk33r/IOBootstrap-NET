import BaseRequestModel from "../../../common/models/BaseRequestModel";

class UserChangePasswordRequestModel extends BaseRequestModel {

    userName: string;
    oldPassword: string | null;
    newPassword: string;

    constructor() {
        super();

        this.userName = "";
        this.oldPassword = null;
        this.newPassword = "";
    }
}

export default UserChangePasswordRequestModel;
