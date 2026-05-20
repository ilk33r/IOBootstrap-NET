import { BaseResponseModel } from "iobootstrap-ui-base";
import FileVariationsModel from "./FileVariationsModel";

class SaveFileResponseModel extends BaseResponseModel {

    file: FileVariationsModel | null;

    constructor() {
        super();

        this.file = null;
    }
}

export default SaveFileResponseModel;
