import { BaseRequestModel } from "iobootstrap-ui-base";

class UpdateUserRequestModel extends BaseRequestModel {

    userId: number;
    userName: string;
    userRole: number;
    isActive: boolean;
    activationEndDate: string | null;

    constructor() {
        super();

        this.userId = 0;
        this.userName = "";
        this.userRole = 0;
        this.isActive = false;
        this.activationEndDate = null;
    }
}

export default UpdateUserRequestModel;
