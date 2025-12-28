import ListPushNotificationMessageResponseModel from "../models/ListPushNotificationMessageResponseModel";
import PushNotificationMessageDeleteRequestModel from "../models/PushNotificationMessageDeleteRequestModel";
import PushNotificationListProops from "../props/PushNotificationListProops";
import PushNotificationListState from "../props/PushNotificationListState";
import React from "react";
import { BOController, BreadcrumbNavigationModel, ListDataFilterTypes, ListDataHeaderModel, ListDataItemModel, ListDataPaginationModel, ListView } from "iobootstrap-bo-base";
import IOListPushNotificationsRequestModel from "../models/IOListPushNotificationsRequestModel";

class PushNotificationListController extends BOController<PushNotificationListProops, PushNotificationListState> {

    private requestModel: IOListPushNotificationsRequestModel;

    constructor(props: PushNotificationListProops) {
        super(props);

        this.requestModel = new IOListPushNotificationsRequestModel();

        this.state = new PushNotificationListState();

        this.deleteDataHandler = this.deleteDataHandler.bind(this);
        this.pageChangeHandler = this.pageChangeHandler.bind(this);
    }

    public componentDidMount?(): void {
        this.appContext.removeObject("pushNotificationDeleteRequest");

        this.requestModel.start = 0;
        this.requestModel.count = 25;
        this.loadMessages();
    }

    private loadMessages() {
        this.indicatorPresenter.present();

        const requestPath = `${import.meta.env.VITE_BACKOFFICE_PUSH_NOTIFICATION_CONTROLLER_NAME}/ListMessages`;
        const weakSelf = this;

        this.service.post(requestPath, this.requestModel, function (response: ListPushNotificationMessageResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                const newState = new PushNotificationListState();
                newState.count = response.count;
                newState.messages = response.messages;

                weakSelf.setState(newState);
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    private pageChangeHandler(start: number, length: number) {
        this.requestModel.start = start;
        this.requestModel.count = length;
        this.loadMessages();
    }

    private deleteDataHandler(index: number) {
        const currentMessage = this.state.messages[index];
        const deleteRequestModel = new PushNotificationMessageDeleteRequestModel();
        deleteRequestModel.id = currentMessage.id;
        
        this.appContext.setObjectForKey("pushNotificationDeleteRequest", deleteRequestModel);
        this.navigateToPage("pushNotificationDelete");
    }

    render() {
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("pushNotificationList", "Push Notification Messages")
        ];

        const headers = [
            ListDataHeaderModel.initialize("ID"),
            ListDataHeaderModel.initializeWithFilter("Device Type", ListDataFilterTypes.Select, [
                "Android Google",
                "Android Huawei",
                "iOS",
                "Generic",
                "Unkown"
            ]),
            ListDataHeaderModel.initializeWithFilter("Category", ListDataFilterTypes.Input, null),
            ListDataHeaderModel.initializeWithFilter("Data", ListDataFilterTypes.Input, null),
            ListDataHeaderModel.initializeWithFilter("Message", ListDataFilterTypes.Input, null),
            ListDataHeaderModel.initializeWithFilter("Title", ListDataFilterTypes.Input, null),
            ListDataHeaderModel.initializeWithFilter("Status", ListDataFilterTypes.Select, ["Completed", "Sending"]),
            ListDataHeaderModel.initialize("Created By"),
            ListDataHeaderModel.initialize("Created Date"),
            ListDataHeaderModel.initialize("Updated Date"),
            ListDataHeaderModel.initialize("Delivered Devices")
        ];

        const items = this.state.messages.map(message => {
            const itemModel = new ListDataItemModel();
            let deviceTypeString;
            if (message.deviceType == 0) {
                deviceTypeString = "Android Google";
            } else if (message.deviceType == 1) {
                deviceTypeString = "Android Huawei"
            } else if (message.deviceType == 2) {
                deviceTypeString = "iOS"
            } else if (message.deviceType == 3) {
                deviceTypeString = "Generic"
            } else {
                deviceTypeString = "Unkown"
            }

            const status = (message.isCompleted === true) ? "Completed" : "Sending";
            const createdDate = (message.createdDate === undefined || message.createdDate === null) ? "-" : new Date(message.createdDate).toLocaleDateString('en-US', { year: 'numeric', day: '2-digit', month: '2-digit' });
            const updateDate = (message.updateDate === undefined || message.updateDate === null) ? "-" : new Date(message.updateDate).toLocaleDateString('en-US', { year: 'numeric', day: '2-digit', month: '2-digit' });

            itemModel.itemList = [
                message.id.toString(),
                deviceTypeString,
                (message.notificationCategory ?? "").RemoveHTML(),
                (message.notificationData ?? "").RemoveHTML(),
                (message.notificationMessage).RemoveHTML(),
                (message.notificationTitle).RemoveHTML(),
                status,
                (message.createdBy ?? "").RemoveHTML(),
                createdDate,
                updateDate,
                message.deliveredDevicesCount?.toString() ?? "-"
            ];

            itemModel.textWraps = [
                false,
                false,
                false,
                true,
                true,
                true,
                false,
                false,
                true,
                true,
                false
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
                    resourceDelete="Delete"
                    resourceEdit="Edit"
                    resourceHome="Home"
                    resourceOptions="Options"
                    resourceSelect=""
                    extras={null}
                    deleteDataHandler={this.deleteDataHandler}
                    updateDataHandler={null}
                    selectDataHandler={null}
                    itemVisibleHandler={null}
                    pagination={pagination} />
            </React.StrictMode>
        );
    }
}

export default PushNotificationListController;
