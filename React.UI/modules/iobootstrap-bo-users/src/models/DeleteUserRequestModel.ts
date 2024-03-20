import { BaseRequestModel } from "iobootstrap-ui-base";

class DeleteUserRequestModel extends BaseRequestModel {

    userId: number;

    constructor() {
        super();

        this.userId = 0;
    }
}

export default DeleteUserRequestModel;
