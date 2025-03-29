import { BaseRequestModel } from "iobootstrap-ui-base";

class AddUserRequestModel extends BaseRequestModel {

    userName: string;
    password: string;
    userRole: number;
    isActive: boolean;
    activationEndDate: string;

    constructor() {
        super();

        this.userName = "";
        this.password = "";
        this.userRole = 0;
        this.isActive = false;
        this.activationEndDate = "";
    }
}

export default AddUserRequestModel;
