import { BaseRequestModel } from "iobootstrap-ui-base";
import ImageVariationsModel from "./ImageVariationsModel";

class SaveImageRequestModel extends BaseRequestModel {

    fileData: string | null;
    sizes: ImageVariationsModel[];

    constructor() {
        super();

        this.fileData = null;
        this.sizes = [];
    }
}

export default SaveImageRequestModel;
