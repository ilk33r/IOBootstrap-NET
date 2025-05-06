import { BaseRequestModel } from "iobootstrap-ui-base";

class UserChangePasswordRequestModel extends BaseRequestModel {

    oldPassword: string | null;
    newPassword: string | null;

    constructor() {
        super();

        this.oldPassword = null;
        this.newPassword = null;
    }
}

export default UserChangePasswordRequestModel;
