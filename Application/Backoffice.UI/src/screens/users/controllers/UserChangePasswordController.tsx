import BaseResponseModel from "../../../common/models/BaseResponseModel";
import BreadcrumbNavigationModel from "../../shared/models/BreadcrumbNavigationModel";
import CalloutTypes from "../../../presentation/constants/CalloutTypes";
import CommonConstants from "../../../common/constants/CommonConstants";
import Controller from "../../../presentation/controllers/Controller";
import FormType from "../../shared/interfaces/FormType";
import FormView from "../../shared/views/FormView";
import React from "react";
import UpdateUserRequestModel from "../models/UpdateUserRequestModel";
import UserChangePasswordRequestModel from "../models/UserChangePasswordRequestModel";
import UserRoles from "../../../common/enumerations/UserRoles";
import ValidationMinLengthRule from "../../../presentation/validations/ValidationMinLengthRule";
import FormTypePasswordProps from "../../shared/props/FormTypePasswordProps";

class UserChangePasswordController extends Controller<{}, {}> {

    private _updateRequest: UpdateUserRequestModel;

    constructor(props: {}) {
        super(props);

        this._updateRequest = this.appContext.objectForKey("usersChangePasswordRequest") as UpdateUserRequestModel;
        if (this._updateRequest == null) {
            this._updateRequest = new UpdateUserRequestModel();
            this._updateRequest.userName = this.storage.stringForKey(CommonConstants.userNameStorageKey) ?? "";
        }

        this.handleFormError = this.handleFormError.bind(this);
        this.handleFormSuccess = this.handleFormSuccess.bind(this);
    }

    handleFormError(errorTitle: string, errorMessage: string) {
        this.calloutPresenter.show(CalloutTypes.danger, errorTitle, errorMessage);
    }

    handleFormSuccess(values: string[]) {
        let currentPassword: string | null;
        let password: string;
        let passwordRepeat: string;

        if (this.appContext.numberForKey(CommonConstants.userRoleStorageKey) === UserRoles.SuperAdmin) {
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

        if (this.appContext.numberForKey(CommonConstants.userRoleStorageKey) === UserRoles.SuperAdmin) {
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
