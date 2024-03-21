import { BaseResponseModel } from "iobootstrap-ui-base";
import ImageVariationsModel from "./ImageVariationsModel";

class SaveImageResponseModel extends BaseResponseModel {

    files: ImageVariationsModel[] | null;
    fileName: string | null;

    constructor() {
        super();

        this.files = [];
        this.fileName = null;
    }
}

export default SaveImageResponseModel;
