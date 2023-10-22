import BaseResponseModel from "../../../common/models/BaseResponseModel";
import BreadcrumbNavigationModel from "../../shared/models/BreadcrumbNavigationModel";
import Controller from "../../../presentation/controllers/Controller";
import MessageDeleteRequestModel from "../models/MessageDeleteRequestModel";
import QuestionView from "../../shared/views/QuestionView";
import React from "react";

class MessagesDeleteController extends Controller<{}, {}> {

    private _deleteRequest: MessageDeleteRequestModel;

    constructor(props: {}) {
        super(props);

        this._deleteRequest = this.appContext.objectForKey("messagesDeleteRequest") as MessageDeleteRequestModel;
        if (this._deleteRequest == null) {
            this.navigateToPage("messagesList");
            return;
        }

        this.handleFormError = this.handleFormError.bind(this);
        this.handleFormSuccess = this.handleFormSuccess.bind(this);
    }

    handleFormError() {
        this.navigateToPage("messagesList");
    }

    handleFormSuccess() {
        this.indicatorPresenter.present();

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_MESSAGES_CONTROLLER_NAME}/DeleteMessagesItem`;
        const request = new MessageDeleteRequestModel();
        request.messageId = this._deleteRequest.messageId;

        const weakSelf = this;
        this.service.post(requestPath, request, function (response: BaseResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                weakSelf.showCalloutAndRedirectToHash("Message has been deleted successfully.", "messagesList");
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    render() {
        if (this._deleteRequest == null) {
            return (<React.StrictMode></React.StrictMode>);
        }
    
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("messagesList", "Messages"),
            BreadcrumbNavigationModel.initialize("messagesDelete", "Delete Message")
        ];

        return (
            <React.StrictMode>
                <QuestionView navigation={navigation} 
                    resourceHome="Home"
                    title="Delete a message"
                    questionMessage="Are you sure want to delete this message ?"
                    errorHandler={this.handleFormError}
                    successHandler={this.handleFormSuccess}
                     />
            </React.StrictMode>
        );
    }
}

export default MessagesDeleteController;
