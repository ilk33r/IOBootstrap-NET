import { BaseRequestModel } from "iobootstrap-ui-base";

class GenerateBOPageRequestModel extends BaseRequestModel {

    entityName: string;

    constructor() {
        super();

        this.entityName = "";
    }
}

export default GenerateBOPageRequestModel;