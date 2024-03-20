import { BaseRequestModel } from "iobootstrap-ui-base";

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
