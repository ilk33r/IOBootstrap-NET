import { BaseResponseModel } from "iobootstrap-ui-base";
import ImageVariationsModel from "./ImageVariationsModel";

class SaveImageResponseModel extends BaseResponseModel {

    file: ImageVariationsModel | null;

    constructor() {
        super();

        this.file = null;
    }
}

export default SaveImageResponseModel;
