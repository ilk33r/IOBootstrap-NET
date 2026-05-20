import { BaseRequestModel } from "iobootstrap-ui-base";

class DeleteFilesRequestModel extends BaseRequestModel {

    fileId: number | null;

    constructor() {
        super();

        this.fileId = null;
    }
}

export default DeleteFilesRequestModel;
