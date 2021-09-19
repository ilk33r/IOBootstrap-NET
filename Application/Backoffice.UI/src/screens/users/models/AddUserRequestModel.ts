import BaseRequestModel from "../../../common/models/BaseRequestModel";

class AddUserRequestModel extends BaseRequestModel {

    userName: string;
    password: string;
    userRole: number;

    constructor() {
        super();

        this.userName = "";
        this.password = "";
        this.userRole = 0;
    }
}

export default AddUserRequestModel;
