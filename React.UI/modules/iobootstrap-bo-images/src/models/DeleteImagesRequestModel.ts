import { BaseRequestModel } from "iobootstrap-ui-base";

class DeleteImagesRequestModel extends BaseRequestModel {

    imagesIdList: number[];

    constructor() {
        super();

        this.imagesIdList = [];
    }
}

export default DeleteImagesRequestModel;
