import GetLogsRequestModel from "../models/GetLogsRequestModel";
import GetLogsResponseModel from "../models/GetLogsResponseModel";
import LogsListProps from "../props/LogsListProps";
import LogsListState from "../props/LogsListState";
import React from "react";
import { BOController, BreadcrumbNavigationModel, ListDataFilterTypes, ListDataHeaderModel, ListDataItemModel, ListDataPaginationModel, ListView } from "iobootstrap-bo-base";

class LogsController extends BOController<LogsListProps, LogsListState> {

    private requestModel: GetLogsRequestModel;

    constructor(props: LogsListProps) {
        super(props);

        this.requestModel = new GetLogsRequestModel();

        this.state = new LogsListState();

        this.pageChangeHandler = this.pageChangeHandler.bind(this);
    }

    private LoadLogs() {
        this.indicatorPresenter.present();

        const requestPath = `${import.meta.env.VITE_BACKOFFICE_LOGS_CONTROLLER_NAME}/GetLogs`;
        const weakSelf = this;

        this.service.post(requestPath, this.requestModel, function (response: GetLogsResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                const newState = new LogsListState();
                newState.logs = response.logs;
                newState.count = response.count;

                weakSelf.setState(newState);
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    public componentDidMount?(): void {
        this.requestModel.start = 0;
        this.requestModel.count = 50;
        this.LoadLogs();
    }

    pageChangeHandler(start: number, length: number) {
        this.requestModel.start = start;
        this.requestModel.count = length;
        this.LoadLogs();
    }

    render() {
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("logsEdit", "Logs")
        ];

        const ips = this.state.logs.map(log => {
            return log.ipV4 ?? "";
        });

        const codes = this.state.logs.map(log => {
            return log.responseCode?.toString() ?? "";
        });

        const headers = [
            ListDataHeaderModel.initialize("ID"),
            ListDataHeaderModel.initialize("Date"),
            ListDataHeaderModel.initializeWithFilter("IP", ListDataFilterTypes.Select, ips),
            ListDataHeaderModel.initializeWithFilter("Path", ListDataFilterTypes.Input, null),
            ListDataHeaderModel.initializeWithFilter("Code", ListDataFilterTypes.Select, codes),
            ListDataHeaderModel.initialize("Request"),
            ListDataHeaderModel.initialize("Response"),
        ];

        const items = this.state.logs.map(log => {
            const itemModel = new ListDataItemModel();
            const logId = (log.id == null) ? "" : log.id.toString();
            const date = (log.requestDate === undefined || log.requestDate === null) ? "-" : new Date(log.requestDate).toLocaleDateString('en-US', { year: 'numeric', day: '2-digit', month: '2-digit', hour: '2-digit', minute: '2-digit' });
            const ip = (log.ipV4 == null) ? "" : log.ipV4;
            const port = (log.port == null) ? "" : log.port.toString();
            const path = (log.requestPath == null) ? "" : log.requestPath;
            const code = (log.responseCode == null) ? "" : log.responseCode.toString();
            const requestHeaders = (log.requestHeaders == null) ? "" : log.requestHeaders;
            const requestBody = (log.requestBody == null) ? "" : log.requestBody;
            const responseHeaders = (log.responseHeaders == null) ? "" : log.responseHeaders;
            const responseBody = (log.responseBody == null) ? "" : log.responseBody;

            itemModel.itemList = [
                logId,
                date,
                `${ip}:${port}`,
                path,
                code,
                `<ul class="list-group"><li class="list-group-item"><h5>Header</h5><p>${requestHeaders}</p></li><li class="list-group-item"><h5>Body</h5><p>${requestBody}</p></li></ul>`,
                `<ul class="list-group"><li class="list-group-item"><h5>Header</h5><p>${responseHeaders}</p></li><li class="list-group-item"><h5>Body</h5><p>${responseBody}</p></li></ul>`
            ];

            itemModel.textWraps = [
                false,
                false,
                false,
                false,
                false,
                true,
                true
            ];

            return itemModel;
        });

        const pagination = new ListDataPaginationModel();
        pagination.start = this.requestModel.start;
        pagination.length = this.requestModel.count;
        pagination.count = this.state.count;
        pagination.pageClickHandler = this.pageChangeHandler;

        return (
            <React.StrictMode>
                <ListView navigation={navigation} 
                    headers={headers} 
                    items={items}
                    resourceDelete=""
                    resourceEdit=""
                    resourceHome="Home"
                    resourceOptions=""
                    resourceSelect=""
                    extras={null}
                    deleteDataHandler={null}
                    updateDataHandler={null}
                    selectDataHandler={null}
                    itemVisibleHandler={null}
                    pagination={pagination} />
            </React.StrictMode>
        );
    }
}

export default LogsController;
