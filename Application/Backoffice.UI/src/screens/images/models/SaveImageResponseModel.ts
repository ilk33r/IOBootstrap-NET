import BaseResponseModel from "../../../common/models/BaseResponseModel";
import ImageVariationsModel from "./ImageVariationsModel";

class SaveImageResponseModel extends BaseResponseModel {

    files: ImageVariationsModel[];

    constructor() {
        super();

        this.files = [];
    }
}

export default SaveImageResponseModel;
