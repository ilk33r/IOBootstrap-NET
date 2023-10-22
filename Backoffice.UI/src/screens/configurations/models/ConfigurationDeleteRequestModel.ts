import BaseRequestModel from "../../../common/models/BaseRequestModel";

class ConfigurationDeleteRequestModel extends BaseRequestModel {

    configId: number;

    constructor() {
        super();
        this.configId = 0;
    }
}

export default ConfigurationDeleteRequestModel;
