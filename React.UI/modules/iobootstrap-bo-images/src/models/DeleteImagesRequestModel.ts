import { BaseRequestModel } from "iobootstrap-ui-base";

class DeleteImagesRequestModel extends BaseRequestModel {

    imageId: number | null;

    constructor() {
        super();

        this.imageId = null;
    }
}

export default DeleteImagesRequestModel;
