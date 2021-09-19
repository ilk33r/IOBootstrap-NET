class ConfigurationModel {

    id: number;
    configKey: string;
    configIntValue: number | null;
    configStringValue: string | null;

    constructor() {
        this.id = 0;
        this.configKey = "";
        this.configIntValue = null;
        this.configStringValue = null;
    }
}

export default ConfigurationModel;
