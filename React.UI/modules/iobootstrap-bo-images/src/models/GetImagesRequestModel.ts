import { BaseRequestModel } from "iobootstrap-ui-base";

class GetImagesRequestModel extends BaseRequestModel {

    start: number;
    count: number;

    constructor() {
        super();

        this.start = 0;
        this.count = 0;
    }
}

export default GetImagesRequestModel;
