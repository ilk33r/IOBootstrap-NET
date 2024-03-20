import { BaseRequestModel } from "iobootstrap-ui-base";

class ConfigurationDeleteRequestModel extends BaseRequestModel {

    configId: number;

    constructor() {
        super();
        this.configId = 0;
    }
}

export default ConfigurationDeleteRequestModel;
