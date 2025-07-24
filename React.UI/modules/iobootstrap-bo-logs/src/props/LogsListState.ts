import LogModel from "../models/LogModel";

class LogsListState {

    count: number;
    logs: LogModel[];

    constructor() {
        this.count = 0;
        this.logs = [];
    }
}

export default LogsListState;
