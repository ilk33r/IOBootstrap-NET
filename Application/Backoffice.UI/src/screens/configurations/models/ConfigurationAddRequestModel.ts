import BaseRequestModel from "../../../common/models/BaseRequestModel";

class ConfigurationAddRequestModel extends BaseRequestModel {

    configKey: string;
    strValue: string | null;
    intValue: number | null;
    
    constructor() {
        super();
        this.configKey = "";
        this.strValue = null;
        this.intValue = null;
    }
}

export default ConfigurationAddRequestModel;
