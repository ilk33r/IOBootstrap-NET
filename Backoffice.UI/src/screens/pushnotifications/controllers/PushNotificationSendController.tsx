import BaseResponseModel from "../../../common/models/BaseResponseModel";
import BreadcrumbNavigationModel from "../../shared/models/BreadcrumbNavigationModel";
import CalloutTypes from "../../../presentation/constants/CalloutTypes";
import Controller from "../../../presentation/controllers/Controller";
import DeviceTypes from "../../../common/enumerations/DeviceTypes";
import FormDataOptionModel from "../../shared/models/FormDataOptionModel";
import FormType from "../../shared/interfaces/FormType";
import FormTypePopupSelectionProps from "../../shared/props/FormTypePopupSelectionProps";
import FormTypeSelectProps from "../../shared/props/FormTypeSelectProps";
import FormTypeTextAreaProps from "../../shared/props/FormTypeTextAreaProps";
import FormTypeTextProps from "../../shared/props/FormTypeTextProps";
import FormView from "../../shared/views/FormView";
import React from "react";
import SendPushNotificationRequestModel from "../models/SendPushNotificationRequestModel";
import ValidationMaxLengthRule from "../../../presentation/validations/ValidationMaxLengthRule";
import ValidationMinLengthRule from "../../../presentation/validations/ValidationMinLengthRule";
import ValidationRequiredRule from "../../../presentation/validations/ValidationRequiredRule";

class PushNotificationSendController extends Controller<{}, {}> {

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
        
        const requestPath = `${process.env.REACT_APP_BACKOFFICE_PUSH_NOTIFICATION_CONTROLLER_NAME}/SendNotification`;
        const request = new SendPushNotificationRequestModel();
        request.deviceType = Number(values[0]);
        request.clientId = Number(values[1]);
        request.notificationCategory = values[2];
        request.notificationTitle = values[3];
        request.notificationMessage = values[4];
        request.notificationData = values[5];

        const weakSelf = this;
        this.service.post(requestPath, request, function (response: BaseResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                weakSelf.showCalloutAndRedirectToHash("Push notification has been send successfully.", "pushNotificationList");
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    render() {
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("pushNotificationList", "Push Notification Messages"),
            BreadcrumbNavigationModel.initialize("pushNotificationSend", "Send Push Notification")
        ];

        const formElements: FormType[] = [
            FormTypeSelectProps.initialize("Device Type", "", true, [
                FormDataOptionModel.initialize(DeviceTypes.getDeviceName(DeviceTypes.Android), DeviceTypes.Android.toString()),
                FormDataOptionModel.initialize(DeviceTypes.getDeviceName(DeviceTypes.iOS), DeviceTypes.iOS.toString()),
                FormDataOptionModel.initialize(DeviceTypes.getDeviceName(DeviceTypes.Generic), DeviceTypes.Generic.toString())
            ]),
            FormTypePopupSelectionProps.initialize("Client", "", 0, "clientsSelect", true),
            FormTypeSelectProps.initialize("Category", "", true, [
                FormDataOptionModel.initialize("-", "")
            ]),
            FormTypeTextProps.initializeWithValidations("Title", "", true, [ ValidationRequiredRule.initialize("Title is too short.", "Invalid notification title."),
                                                                                ValidationMaxLengthRule.initialize("Title is too long.", "Title must be smaller than 32 characters.", 32) ]),
            FormTypeTextProps.initializeWithValidations("Message", "", true, [ ValidationMinLengthRule.initialize("Message is too short.", "Invalid notification message.", 3),
                                                                                ValidationMaxLengthRule.initialize("Message is too long.", "Message must be smaller than 256 characters.", 256) ]),
            FormTypeTextAreaProps.initialize("Custom Data", "", true)
        ];

        return (
            <React.StrictMode>
                <FormView navigation={navigation} 
                    resourceHome="Home"
                    title="Send a push notification"
                    submitButtonName="Send"
                    errorHandler={this.handleFormError}
                    successHandler={this.handleFormSuccess}
                    formElements={formElements} />
            </React.StrictMode>
        );
    }
}

export default PushNotificationSendController;
