import BaseResponseModel from "../../../common/models/BaseResponseModel";
import BreadcrumbNavigationModel from "../../shared/models/BreadcrumbNavigationModel";
import CalloutTypes from "../../../presentation/constants/CalloutTypes";
import Controller from "../../../presentation/controllers/Controller";
import FormType from "../../shared/interfaces/FormType";
import FormTypeTextAreaProps from "../../shared/props/FormTypeTextAreaProps";
import FormView from "../../shared/views/FormView";
import MessageUpdateRequestModel from "../models/MessageUpdateRequestModel";
import React from "react";
import ValidationMinLengthRule from "../../../presentation/validations/ValidationMinLengthRule";
import FormTypeDateProps from "../../shared/props/FormTypeDateProps";

class MessagesUpdateController extends Controller<{}, {}> {

    private _updateRequest: MessageUpdateRequestModel;

    constructor(props: {}) {
        super(props);

        this._updateRequest = this.appContext.objectForKey("messagesUpdateRequest") as MessageUpdateRequestModel;
        if (this._updateRequest == null) {
            this.navigateToPage("messagesList");
            return;
        }

        this.handleFormError = this.handleFormError.bind(this);
        this.handleFormSuccess = this.handleFormSuccess.bind(this);
    }

    private formatDate(date: Date): string {
        const dateMonthValue = date.getMonth() + 1;
        const dateMonth = (dateMonthValue < 10) ? '0' + dateMonthValue.toString() : dateMonthValue.toString();

        const dateDayValue = date.getDate();
        const dateDay = (dateDayValue < 10) ? '0' + dateDayValue.toString() : dateDayValue.toString();

        return date.getFullYear() + '-' + dateMonth + '-' + dateDay;        
    }

    handleFormError(errorTitle: string, errorMessage: string) {
        this.calloutPresenter.show(CalloutTypes.danger, errorTitle, errorMessage);
    }

    handleFormSuccess(values: string[]) {
        this.indicatorPresenter.present();

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_MESSAGES_CONTROLLER_NAME}/UpdateMessagesItem`;
        const request = new MessageUpdateRequestModel();
        request.messageId = this._updateRequest.messageId;
        request.message = values[0];
        request.messageStartDate = values[1];
        request.messageEndDate = values[2];

        const weakSelf = this;
        this.service.post(requestPath, request, function (response: BaseResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                weakSelf.showCalloutAndRedirectToHash("Message has been updated successfully.", "messagesList");
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    render() {
        if (this._updateRequest == null) {
            return (<React.StrictMode></React.StrictMode>);
        }
    
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("messagesList", "Messages"),
            BreadcrumbNavigationModel.initialize("messagesUpdate", "Update Message")
        ];

        let startDateString = "";
        if (this._updateRequest.messageStartDate != null) {
            const startDate = new Date(this._updateRequest.messageStartDate);
            startDateString = this.formatDate(startDate);
        }

        let endDateString = "";
        if (this._updateRequest.messageEndDate != null) {
            const endDate = new Date(this._updateRequest.messageEndDate);
            endDateString = this.formatDate(endDate);
        }

        const formElements: FormType[] = [
            FormTypeTextAreaProps.initializeWithValidations("Message", this._updateRequest.message, true, [ ValidationMinLengthRule.initialize("Message is too short.", "Invalid message.", 3) ]),
            FormTypeDateProps.initialize("Start Date", startDateString, true),
            FormTypeDateProps.initialize("End Date", endDateString, true)
        ];

        return (
            <React.StrictMode>
                <FormView navigation={navigation} 
                    resourceHome="Home"
                    title="Update a message"
                    submitButtonName="Save"
                    errorHandler={this.handleFormError}
                    successHandler={this.handleFormSuccess}
                    formElements={formElements} />
            </React.StrictMode>
        );
    }
}

export default MessagesUpdateController;
