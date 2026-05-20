import { BaseResponseModel } from "iobootstrap-ui-base";
import FileVariationsModel from "./FileVariationsModel";

class GetFilesResponseModel extends BaseResponseModel {

    count: number;
    files: FileVariationsModel[];

    constructor() {
        super();

        this.count = 0;
        this.files = [];
    }
}

export default GetFilesResponseModel;
