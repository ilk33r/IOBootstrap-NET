import ConfigurationModel from "../models/ConfigurationModel";

class ConfigurationListState {

    configurations: ConfigurationModel[];

    constructor() {
        this.configurations = [];
    }
}

export default ConfigurationListState;
