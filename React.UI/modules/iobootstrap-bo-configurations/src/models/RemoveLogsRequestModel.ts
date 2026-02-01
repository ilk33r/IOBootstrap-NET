import { BaseRequestModel } from "iobootstrap-ui-base";

class RemoveLogsRequestModel extends BaseRequestModel {

    startDate: string | null;

    constructor() {
        super();

        this.startDate = null;
    }
}

export default RemoveLogsRequestModel;