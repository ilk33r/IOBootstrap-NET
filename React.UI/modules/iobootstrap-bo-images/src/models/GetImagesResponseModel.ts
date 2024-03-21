import { BaseResponseModel } from "iobootstrap-ui-base";
import ImageVariationsModel from "./ImageVariationsModel";

class GetImagesResponseModel extends BaseResponseModel {

    count: number;
    images: ImageVariationsModel[];

    constructor() {
        super();

        this.count = 0;
        this.images = [];
    }
}

export default GetImagesResponseModel;
