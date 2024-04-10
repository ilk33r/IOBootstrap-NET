import { BaseRequestModel } from "iobootstrap-ui-base";

class UpdateUserRequestModel extends BaseRequestModel {

    userId: number;
    userName: string;
    userRole: number;

    constructor() {
        super();

        this.userId = 0;
        this.userName = "";
        this.userRole = 0;
    }
}

export default UpdateUserRequestModel;
