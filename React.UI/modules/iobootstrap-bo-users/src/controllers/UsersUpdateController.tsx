import React from "react";
import UpdateUserRequestModel from "../models/UpdateUserRequestModel";
import { BaseResponseModel, CalloutTypes, DIHooks, ValidationBackofficeRequestRule, ValidationDateRule, ValidationMinLengthRule, ValidationRequiredRule } from "iobootstrap-ui-base";
import { BOController, BreadcrumbNavigationModel, FormDataOptionModel, FormType, FormTypeDateProps, FormTypeSelectProps, FormTypeTextProps, FormView } from "iobootstrap-bo-base";

class UsersUpdateController extends BOController<{}, {}> {

    private _updateRequest: UpdateUserRequestModel;

    constructor(props: {}) {
        super(props);

        this._updateRequest = this.appContext.objectForKey("usersUpdateRequest") as UpdateUserRequestModel;
        if (this._updateRequest == null) {
            this.navigateToPage("usersList");
            return;
        }

        this.handleFormError = this.handleFormError.bind(this);
        this.handleFormSuccess = this.handleFormSuccess.bind(this);
    }

    handleFormError(errorTitle: string, errorMessage: string) {
        this.calloutPresenter.show(CalloutTypes.danger, errorTitle, errorMessage);
    }

    handleFormSuccess(values: string[], blobs: Blob[]) {
        this.indicatorPresenter.present();

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_USER_CONTROLLER_NAME}/UpdateUser`;
        const request = new UpdateUserRequestModel();
        request.userId = this._updateRequest.userId;
        request.userName = values[0];
        request.userRole = Number(values[1]);
        request.isActive = (values[2] == "yes") ? true : false;
        request.activationEndDate = values[3];

        const weakSelf = this;
        this.service.post(requestPath, request, function (response: BaseResponseModel) {
            if (response.status !== undefined && response.status.code === 700) {
                const helpText = 'User ' + request.userName + ' is exists.';
                weakSelf.indicatorPresenter.dismiss();
                weakSelf.calloutPresenter.show(CalloutTypes.danger, "Invalid username.", helpText);
                return;
            }

            if (weakSelf.handleServiceSuccess(response)) {
                weakSelf.showCalloutAndRedirectToHash("User has been updated successfully.", "usersList");
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
            BreadcrumbNavigationModel.initialize("usersList", "Users"),
            BreadcrumbNavigationModel.initialize("usersUpdate", "Update User")
        ];

        let userRoleFormDataOptions: FormDataOptionModel[] = []
        const userRoleFormDataOptionsHook = DIHooks.Instance.hookForKey("userRoleFormDataOptions")
        if (userRoleFormDataOptionsHook != null) {
            const userRoleFormDataOptionsAny = userRoleFormDataOptionsHook(null);
            if (userRoleFormDataOptionsAny != null) {
                userRoleFormDataOptions = userRoleFormDataOptionsAny;
            }
        }
        
        const activationEndDate = (this._updateRequest.activationEndDate === null) ? "" : this.formatDate(new Date(this._updateRequest.activationEndDate));
        const formElements: FormType[] = [
            FormTypeTextProps.initializeWithValidations("User Name", this._updateRequest.userName, true, [ 
                ValidationMinLengthRule.initialize("User name is too short.", "Invalid user name.", 3),
                ValidationBackofficeRequestRule.initialize("Invalid characters.", "Invalid characters.")
            ]),
            FormTypeSelectProps.initialize("Role", this._updateRequest.userRole.toString(), true, userRoleFormDataOptions),
            FormTypeSelectProps.initialize("Active", this._updateRequest.isActive ? "yes" : "no", true, [ 
                FormDataOptionModel.initialize("NO", "no"),
                FormDataOptionModel.initialize("YES", "yes")
            ]),
            FormTypeDateProps.initializeWithValidations("End Date", activationEndDate, true, [ 
                ValidationRequiredRule.initialize("End date is required.", "Invalid end date."),
                ValidationDateRule.initialize("Invalid date.", "Invalid date.")
            ])
        ];

        return (
            <React.StrictMode>
                <FormView navigation={navigation} 
                    resourceHome="Home"
                    title="Update a user"
                    submitButtonName="Save"
                    errorHandler={this.handleFormError}
                    successHandler={this.handleFormSuccess}
                    formElements={formElements} />
            </React.StrictMode>
        );
    }

    private formatDate(date: Date): string {
        const dateMonthValue = date.getMonth() + 1;
        const dateMonth = (dateMonthValue < 10) ? '0' + dateMonthValue.toString() : dateMonthValue.toString();

        const dateDayValue = date.getDate();
        const dateDay = (dateDayValue < 10) ? '0' + dateDayValue.toString() : dateDayValue.toString();

        return date.getFullYear() + '-' + dateMonth + '-' + dateDay;        
    }
}

export default UsersUpdateController;
