import BaseRequestModel from "../../../common/models/BaseRequestModel";

class GenerateBOPageRequestModel extends BaseRequestModel {

    entityName: string;

    constructor() {
        super();

        this.entityName = "";
    }
}

export default GenerateBOPageRequestModel;