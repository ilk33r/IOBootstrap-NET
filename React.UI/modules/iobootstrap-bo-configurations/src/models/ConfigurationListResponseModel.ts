import { BaseResponseModel } from "iobootstrap-ui-base";
import ConfigurationModel from "./ConfigurationModel";

class ConfigurationListResponseModel extends BaseResponseModel {

    configurations: ConfigurationModel[];

    constructor() {
        super();
        this.configurations = [];
    }
}

export default ConfigurationListResponseModel;
