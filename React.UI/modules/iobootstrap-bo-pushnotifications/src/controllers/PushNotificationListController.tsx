import ListPushNotificationMessageResponseModel from "../models/ListPushNotificationMessageResponseModel";
import PushNotificationMessageDeleteRequestModel from "../models/PushNotificationMessageDeleteRequestModel";
import PushNotificationListProops from "../props/PushNotificationListProops";
import PushNotificationListState from "../props/PushNotificationListState";
import React from "react";
import { BOController, BreadcrumbNavigationModel, ListDataFilterTypes, ListDataHeaderModel, ListDataItemModel, ListView } from "iobootstrap-bo-base";

class PushNotificationListController extends BOController<PushNotificationListProops, PushNotificationListState> {

    constructor(props: PushNotificationListProops) {
        super(props);

        this.state = new PushNotificationListState();

        this.deleteDataHandler = this.deleteDataHandler.bind(this);
    }

    public componentDidMount?(): void {
        this.appContext.removeObject("pushNotificationDeleteRequest");

        this.indicatorPresenter.present();

        const requestPath = `${import.meta.env.VITE_BACKOFFICE_PUSH_NOTIFICATION_CONTROLLER_NAME}/ListMessages`;
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

        const headers = [
            ListDataHeaderModel.initialize("ID"),
            ListDataHeaderModel.initialize("Date"),
            ListDataHeaderModel.initializeWithFilter("Category", ListDataFilterTypes.Input, null),
            ListDataHeaderModel.initializeWithFilter("Message Data", ListDataFilterTypes.Input, null),
            ListDataHeaderModel.initializeWithFilter("Message", ListDataFilterTypes.Input, null),
            ListDataHeaderModel.initializeWithFilter("Title", ListDataFilterTypes.Input, null),
            ListDataHeaderModel.initializeWithFilter("Status", ListDataFilterTypes.Select, ["Completed", "Sending"]),
        ];

        const items = this.state.messages.map(message => {
            const itemModel = new ListDataItemModel();
            const notificationDate = new Date(message.notificationDate);
            const status = (message.isCompleted === true) ? "Completed" : "Sending";

            itemModel.itemList = [
                message.id.toString(),
                notificationDate.toLocaleDateString('en-US', { year: 'numeric', day: '2-digit', month: '2-digit' }),
                (message.notificationCategory ?? "").RemoveHTML(),
                (message.notificationData ?? "").RemoveHTML(),
                (message.notificationMessage).RemoveHTML(),
                (message.notificationTitle).RemoveHTML(),
                status
            ];

            return itemModel;
        });

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
                    pagination={null} />
            </React.StrictMode>
        );
    }
}

export default PushNotificationListController;
