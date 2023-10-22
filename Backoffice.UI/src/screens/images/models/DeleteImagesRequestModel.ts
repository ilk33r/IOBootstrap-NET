import BaseRequestModel from "../../../common/models/BaseRequestModel";

class DeleteImagesRequestModel extends BaseRequestModel {

    imagesIdList: number[];

    constructor() {
        super();

        this.imagesIdList = [];
    }
}

export default DeleteImagesRequestModel;
