import ListPushNotificationMessageResponseModel from "../models/ListPushNotificationMessageResponseModel";
import PushNotificationMessageDeleteRequestModel from "../models/PushNotificationMessageDeleteRequestModel";
import PushNotificationListProops from "../props/PushNotificationListProops";
import PushNotificationListState from "../props/PushNotificationListState";
import React from "react";
import { Controller } from "iobootstrap-ui-base";
import { BreadcrumbNavigationModel, ListDataItemModel, ListView } from "iobootstrap-bo-base";

class PushNotificationListController extends Controller<PushNotificationListProops, PushNotificationListState> {

    constructor(props: PushNotificationListProops) {
        super(props);

        this.state = new PushNotificationListState();

        this.deleteDataHandler = this.deleteDataHandler.bind(this);
    }

    public componentDidMount?(): void {
        this.appContext.removeObject("pushNotificationDeleteRequest");

        this.indicatorPresenter.present();

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_PUSH_NOTIFICATION_CONTROLLER_NAME}/ListMessages`;
        const weakSelf = this;

        this.service.get(requestPath, function (response: ListPushNotificationMessageResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                const newState = new PushNotificationListState();
                newState.messages = response.messages;

                weakSelf.setState(newState);
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    deleteDataHandler(index: number) {
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

        const listDataHeaders = [
            'ID',
            'Client',
            'Date',
            'Category',
            'Message Data',
            'Message',
            'Title',
            'Status'
        ];

        const items = this.state.messages.map(message => {
            const itemModel = new ListDataItemModel();
            const clientDescription = (message.client != null) ? message.client.clientDescription : "";
            const notificationDate = new Date(message.notificationDate);
            const status = (message.isCompleted === true) ? "Completed" : "Sending";

            itemModel.itemList = [
                message.id.toString(),
                clientDescription,
                notificationDate.toLocaleDateString(),
                message.notificationCategory ?? "",
                message.notificationData ?? "",
                message.notificationMessage,
                message.notificationTitle,
                status
            ];

            return itemModel;
        });

        return (
            <React.StrictMode>
                <ListView navigation={navigation} 
                    listDataHeaders={listDataHeaders} 
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
                    pagination={null} />
            </React.StrictMode>
        );
    }
}

export default PushNotificationListController;
