import BaseRequestModel from "../../../common/models/BaseRequestModel";

class DeleteUserRequestModel extends BaseRequestModel {

    userId: number;

    constructor() {
        super();

        this.userId = 0;
    }
}

export default DeleteUserRequestModel;
