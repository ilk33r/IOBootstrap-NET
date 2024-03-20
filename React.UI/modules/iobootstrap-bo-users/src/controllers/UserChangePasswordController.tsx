import React from "react";
import UpdateUserRequestModel from "../models/UpdateUserRequestModel";
import UserChangePasswordRequestModel from "../models/UserChangePasswordRequestModel";
import { BaseResponseModel, CalloutTypes, Controller, ValidationMinLengthRule } from "iobootstrap-ui-base";
import { BOCommonConstants, BreadcrumbNavigationModel, FormType, FormTypePasswordProps, FormView, UserRoles } from "iobootstrap-bo-base";

class UserChangePasswordController extends Controller<{}, {}> {

    private _updateRequest: UpdateUserRequestModel;

    constructor(props: {}) {
        super(props);

        this._updateRequest = this.appContext.objectForKey("usersChangePasswordRequest") as UpdateUserRequestModel;
        if (this._updateRequest == null) {
            this._updateRequest = new UpdateUserRequestModel();
            this._updateRequest.userName = this.storage.stringForKey(BOCommonConstants.userNameStorageKey) ?? "";
        }

        this.handleFormError = this.handleFormError.bind(this);
        this.handleFormSuccess = this.handleFormSuccess.bind(this);
    }

    handleFormError(errorTitle: string, errorMessage: string) {
        this.calloutPresenter.show(CalloutTypes.danger, errorTitle, errorMessage);
    }

    handleFormSuccess(values: string[], blobs: Blob[]) {
        let currentPassword: string | null;
        let password: string;
        let passwordRepeat: string;

        if (this.appContext.numberForKey(BOCommonConstants.userRoleStorageKey) === UserRoles.SuperAdmin) {
            currentPassword = null;
            password = values[0];
            passwordRepeat = values[1];
        } else {
            currentPassword = values[0];
            password = values[1];
            passwordRepeat = values[2];
        }

        if (password !== passwordRepeat) {
            this.handleFormError("Passwords did not match.", "Invalid password.");
            return;
        }

        this.indicatorPresenter.present();

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_USER_CONTROLLER_NAME}/ChangePassword`;
        const request = new UserChangePasswordRequestModel();
        request.userName = this._updateRequest.userName;
        request.oldPassword = currentPassword;
        request.newPassword = password;

        const weakSelf = this;
        this.service.post(requestPath, request, function (response: BaseResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                weakSelf.showCalloutAndRedirectToHash("User password has been changed successfully.", "usersList");
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
            BreadcrumbNavigationModel.initialize("userChangePassword", "Change Password")
        ];

        let formElements: FormType[];

        if (this.appContext.numberForKey(BOCommonConstants.userRoleStorageKey) === UserRoles.SuperAdmin) {
            formElements = [
                FormTypePasswordProps.initializeWithValidations("Password", "", true, [ ValidationMinLengthRule.initialize("Password is too short.", "Invalid password.", 3) ]),
                FormTypePasswordProps.initializeWithValidations("Password (Repeat)", "", true, [ ValidationMinLengthRule.initialize("Password is too short.", "Invalid password.", 3) ])
            ];
        } else {
            formElements = [
                FormTypePasswordProps.initializeWithValidations("Current Password", "", true, [ ValidationMinLengthRule.initialize("Password is too short.", "Invalid password.", 3) ]),
                FormTypePasswordProps.initializeWithValidations("Password", "", true, [ ValidationMinLengthRule.initialize("Password is too short.", "Invalid password.", 3) ]),
                FormTypePasswordProps.initializeWithValidations("Password (Repeat)", "", true, [ ValidationMinLengthRule.initialize("Password is too short.", "Invalid password.", 3) ])
            ];
        }

        return (
            <React.StrictMode>
                <FormView navigation={navigation} 
                    resourceHome="Home"
                    title="Change password"
                    submitButtonName="Save"
                    errorHandler={this.handleFormError}
                    successHandler={this.handleFormSuccess}
                    formElements={formElements} />
            </React.StrictMode>
        );
    }
}

export default UserChangePasswordController;
