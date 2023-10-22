import BaseResponseModel from "../../../common/models/BaseResponseModel";
import BreadcrumbNavigationModel from "../../shared/models/BreadcrumbNavigationModel";
import CalloutTypes from "../../../presentation/constants/CalloutTypes";
import Controller from "../../../presentation/controllers/Controller";
import FormType from "../../shared/interfaces/FormType";
import FormTypeDateProps from "../../shared/props/FormTypeDateProps";
import FormTypeTextAreaProps from "../../shared/props/FormTypeTextAreaProps";
import FormView from "../../shared/views/FormView";
import MessageAddRequestModel from "../models/MessageAddRequestModel";
import React from "react";
import ValidationMinLengthRule from "../../../presentation/validations/ValidationMinLengthRule";

class MessagesAddController extends Controller<{}, {}> {

    constructor(props: {}) {
        super(props);

        this.handleFormError = this.handleFormError.bind(this);
        this.handleFormSuccess = this.handleFormSuccess.bind(this);
    }

    handleFormError(errorTitle: string, errorMessage: string) {
        this.calloutPresenter.show(CalloutTypes.danger, errorTitle, errorMessage);
    }

    handleFormSuccess(values: string[]) {
        this.indicatorPresenter.present();
        
        const requestPath = `${process.env.REACT_APP_BACKOFFICE_MESSAGES_CONTROLLER_NAME}/AddMessagesItem`;
        const request = new MessageAddRequestModel();
        request.message = values[0];
        request.messageStartDate = values[1];
        request.messageEndDate = values[2];

        const weakSelf = this;
        this.service.post(requestPath, request, function (response: BaseResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                weakSelf.showCalloutAndRedirectToHash("Message has been added successfully.", "messagesList");
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    render() {
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("messagesList", "Messages"),
            BreadcrumbNavigationModel.initialize("messagesAdd", "Add Message")
        ];

        const formElements: FormType[] = [
            FormTypeTextAreaProps.initializeWithValidations("Message", "", true, [ ValidationMinLengthRule.initialize("Message is too short.", "Invalid message.", 3) ]),
            FormTypeDateProps.initialize("Start Date", "", true),
            FormTypeDateProps.initialize("End Date", "", true)
        ];

        return (
            <React.StrictMode>
                <FormView navigation={navigation} 
                    resourceHome="Home"
                    title="Add a new message"
                    submitButtonName="Add"
                    errorHandler={this.handleFormError}
                    successHandler={this.handleFormSuccess}
                    formElements={formElements} />
            </React.StrictMode>
        );
    }
}

export default MessagesAddController;
