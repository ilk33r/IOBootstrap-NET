import BaseResponseModel from "../../../common/models/BaseResponseModel";
import ConfigurationModel from "./ConfigurationModel";

class ConfigurationListResponseModel extends BaseResponseModel {

    configurations: ConfigurationModel[];

    constructor() {
        super();
        this.configurations = [];
    }
}

export default ConfigurationListResponseModel;
