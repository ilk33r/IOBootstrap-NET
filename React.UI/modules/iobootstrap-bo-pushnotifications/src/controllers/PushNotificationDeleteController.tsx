import PushNotificationMessageDeleteRequestModel from "../models/PushNotificationMessageDeleteRequestModel";
import React from "react";
import { BaseResponseModel, Controller } from "iobootstrap-ui-base";
import { BreadcrumbNavigationModel, QuestionView } from "iobootstrap-bo-base";

class PushNotificationDeleteController extends Controller<{}, {}> {

    private _deleteRequest: PushNotificationMessageDeleteRequestModel;

    constructor(props: {}) {
        super(props);

        this._deleteRequest = this.appContext.objectForKey("pushNotificationDeleteRequest") as PushNotificationMessageDeleteRequestModel;
        if (this._deleteRequest == null) {
            this.navigateToPage("pushNotificationList");
            return;
        }

        this.handleFormError = this.handleFormError.bind(this);
        this.handleFormSuccess = this.handleFormSuccess.bind(this);
    }

    handleFormError() {
        this.navigateToPage("pushNotificationList");
    }

    handleFormSuccess() {
        this.indicatorPresenter.present();

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_PUSH_NOTIFICATION_CONTROLLER_NAME}/DeleteMessage`;
        const request = new PushNotificationMessageDeleteRequestModel();
        request.id = this._deleteRequest.id;

        const weakSelf = this;
        this.service.post(requestPath, request, function (response: BaseResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                weakSelf.showCalloutAndRedirectToHash("Message has been deleted successfully.", "pushNotificationList");
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
            BreadcrumbNavigationModel.initialize("pushNotificationList", "Push Notification Messages"),
            BreadcrumbNavigationModel.initialize("pushNotificationDelete", "Push Notification Delete")
        ];

        return (
            <React.StrictMode>
                <QuestionView navigation={navigation} 
                    resourceHome="Home"
                    title="Delete a push notification message"
                    questionMessage="Are you sure want to delete this push notification message ?"
                    errorHandler={this.handleFormError}
                    successHandler={this.handleFormSuccess}
                     />
            </React.StrictMode>
        );
    }
}

export default PushNotificationDeleteController;
