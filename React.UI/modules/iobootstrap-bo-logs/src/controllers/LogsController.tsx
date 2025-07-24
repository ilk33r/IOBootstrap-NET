import GetLogsRequestModel from "../models/GetLogsRequestModel";
import GetLogsResponseModel from "../models/GetLogsResponseModel";
import LogsListProps from "../props/LogsListProps";
import LogsListState from "../props/LogsListState";
import React from "react";
import { BOController, BreadcrumbNavigationModel, ListDataItemModel, ListDataPaginationModel, ListView } from "iobootstrap-bo-base";

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

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_LOGS_CONTROLLER_NAME}/GetLogs`;
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

        const listDataHeaders = [
            'ID',
            'Date',
            'IP',
            'Path',
            'Code',
            'Request',
            'Response'
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
                `<div class="list-group"><h4 class="list-group-item-heading">Header</h4><p class="list-group-item-text">${requestHeaders}</p><h4 class="list-group-item-heading">Body</h4><p class="list-group-item-text">${requestBody}</p></div>`,
                `<div class="list-group"><h4 class="list-group-item-heading">Header</h4><p class="list-group-item-text">${responseHeaders}</p><h4 class="list-group-item-heading">Body</h4><p class="list-group-item-text">${responseBody}</p></div>`
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
                    listDataHeaders={listDataHeaders} 
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
