import { BaseRequestModel } from "iobootstrap-ui-base";

class UserResetPasswordRequestModel extends BaseRequestModel {

    userName: string;
    newPassword: string;

    constructor() {
        super();

        this.userName = "";
        this.newPassword = "";
    }
}

export default UserResetPasswordRequestModel;
