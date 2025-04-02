import { BaseRequestModel } from "iobootstrap-ui-base";

class UserChangePasswordRequestModel extends BaseRequestModel {

    oldPassword: string | null;
    newPassword: string;

    constructor() {
        super();

        this.oldPassword = null;
        this.newPassword = "";
    }
}

export default UserChangePasswordRequestModel;
