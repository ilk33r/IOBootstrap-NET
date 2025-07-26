import { BaseRequestModel } from "iobootstrap-ui-base";

class UserResetPasswordRequestModel extends BaseRequestModel {

    userName: string;
    newPassword: string | null;

    constructor() {
        super();

        this.userName = "";
        this.newPassword = null;
    }
}

export default UserResetPasswordRequestModel;
