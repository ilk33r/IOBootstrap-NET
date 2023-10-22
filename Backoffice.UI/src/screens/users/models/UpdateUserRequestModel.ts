import BaseRequestModel from "../../../common/models/BaseRequestModel";

class UpdateUserRequestModel extends BaseRequestModel {

    userId: number;
    userName: string;
    userRole: number;
    userPassword: string | null;

    constructor() {
        super();

        this.userId = 0;
        this.userName = "";
        this.userRole = 0;
        this.userPassword = null;
    }
}

export default UpdateUserRequestModel;
