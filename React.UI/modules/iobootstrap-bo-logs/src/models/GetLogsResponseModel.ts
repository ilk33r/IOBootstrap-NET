import { BaseResponseModel } from "iobootstrap-ui-base";
import LogModel from "./LogModel";

class GetLogsResponseModel extends BaseResponseModel {

    count: number;
    logs: LogModel[];

    constructor() {
        super();

        this.count = 0;
        this.logs = [];
    }
}

export default GetLogsResponseModel;
