import ConfigurationAddRequestModel from "./ConfigurationAddRequestModel";

class ConfigurationUpdateRequestModel extends ConfigurationAddRequestModel {

    configId: number;

    constructor() {
        super();

        this.configId = 0;
    }
}

export default ConfigurationUpdateRequestModel;
