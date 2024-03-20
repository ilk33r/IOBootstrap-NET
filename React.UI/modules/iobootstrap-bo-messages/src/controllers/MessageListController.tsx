import MessageDeleteRequestModel from "../models/MessageDeleteRequestModel";
import MessageListProps from "../props/MessageListProps";
import MessageListResponseModel from "../models/MessageListResponseModel";
import MessageListState from "../props/MessageListState";
import MessageUpdateRequestModel from "../models/MessageUpdateRequestModel";
import React from "react";
import { Controller } from "iobootstrap-ui-base";
import { BreadcrumbNavigationModel, ListDataItemModel, ListView } from "iobootstrap-bo-base";

class MessageListController extends Controller<MessageListProps, MessageListState> {

    constructor(props: MessageListProps) {
        super(props);

        this.state = new MessageListState();

        this.deleteDataHandler = this.deleteDataHandler.bind(this);
        this.updateDataHandler = this.updateDataHandler.bind(this);
    }

    public componentDidMount?(): void {
        this.appContext.removeObject("messagesDeleteRequest");
        this.appContext.removeObject("messagesUpdateRequest");

        this.indicatorPresenter.present();

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_MESSAGES_CONTROLLER_NAME}/ListAllMessages`;
        const weakSelf = this;

        this.service.get(requestPath, function (response: MessageListResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                const newState = new MessageListState();
                newState.messages = response.messages;

                weakSelf.setState(newState);
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    deleteDataHandler(index: number) {
        const selectedMessage = this.state.messages[index];
        const deleteRequestModel = new MessageDeleteRequestModel();
        deleteRequestModel.messageId = selectedMessage.id;
        
        this.appContext.setObjectForKey("messagesDeleteRequest", deleteRequestModel);
        this.navigateToPage("messagesDelete");
    }

    updateDataHandler(index: number) {
        const selectedMessage = this.state.messages[index];
        const updateRequestModel = new MessageUpdateRequestModel();
        updateRequestModel.messageId = selectedMessage.id;
        updateRequestModel.message = selectedMessage.message;
        updateRequestModel.messageStartDate = selectedMessage.messageStartDate;
        updateRequestModel.messageEndDate = selectedMessage.messageEndDate;
        this.appContext.setObjectForKey("messagesUpdateRequest", updateRequestModel);
        this.navigateToPage("messagesUpdate");
    }

    render() {
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("messagesList", "Messages")
        ];

        const listDataHeaders = [
            'ID',
            'Message',
            'Create Date',
            'Start Date',
            'End Date'
        ];

        let items: ListDataItemModel[] = [];

        this.state.messages.forEach(message => {
            const itemModel = new ListDataItemModel();

            const createDate = (message.messageCreateDate == null) ? new Date() : new Date(message.messageCreateDate);
            const startDate = (message.messageStartDate == null) ? new Date() : new Date(message.messageStartDate);
            const endDate = (message.messageEndDate == null) ? new Date() : new Date(message.messageEndDate);

            itemModel.itemList = [
                message.id.toString(),
                message.message.replace(/\n/g, "<br />"),
                createDate.toLocaleDateString(),
                startDate.toLocaleDateString(),
                endDate.toLocaleDateString()
            ];

            items.push(itemModel);
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
                    updateDataHandler={this.updateDataHandler}
                    selectDataHandler={null}
                    pagination={null} />
            </React.StrictMode>
        );
    }
}

export default MessageListController;
