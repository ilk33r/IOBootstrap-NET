import React from "react";
import UpdateUserRequestModel from "../models/UpdateUserRequestModel";
import UserChangePasswordRequestModel from "../models/UserChangePasswordRequestModel";
import { AppCryptography, AppServiceHeaderAuthenticationInterceptor, BaseResponseModel, CalloutTypes, DIHooks, UICommonConstants, ValidationMinLengthRule } from "iobootstrap-ui-base";
import { BOCommonConstants, BOController, BreadcrumbNavigationModel, FormType, FormTypePasswordProps, FormView } from "iobootstrap-bo-base";

class UserChangePasswordController extends BOController<{}, {}> {

    private appServiceHeaderInterceptor: AppServiceHeaderAuthenticationInterceptor;
    private _updateRequest: UpdateUserRequestModel;

    constructor(props: {}) {
        super(props);

        this.appServiceHeaderInterceptor = DIHooks.Instance.singletonForKey("appServiceHeaderInterceptor");
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
        const currentPassword: string = values[0];
        const password: string = values[1];
        const passwordRepeat: string = values[2];

        if (password !== passwordRepeat) {
            this.handleFormError("Passwords did not match.", "Invalid password.");
            return;
        }

        this.indicatorPresenter.present();

        const weakSelf = this;
        this.changePassword(currentPassword, password)
            .catch(() => {
                weakSelf.handleServiceError("", "Cryptography error.");
            });
    }

    private async changePassword(currentPassword: string | null, password: string): Promise<any> {
        const symmetricKeys = await AppCryptography.Instance.getSymmetricKeys();
        this.appServiceHeaderInterceptor.setSymmetricKeys(symmetricKeys.symmetricKey, symmetricKeys.symmetricIV);

        let encryptedCurrentPassword: string | null = null;
        if (currentPassword !== null) {
            encryptedCurrentPassword = await AppCryptography.Instance.encrypt(currentPassword)
        }

        const encryptedNewPassword = await AppCryptography.Instance.encrypt(password);

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_USER_CONTROLLER_NAME}/ChangePassword`;
        const request = new UserChangePasswordRequestModel();
        request.oldPassword = encryptedCurrentPassword;
        request.newPassword = encryptedNewPassword;

        const weakSelf = this;
        this.service.post(requestPath, request, function (response: BaseResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                weakSelf.showCalloutAndRedirectToHash("User password has been changed successfully.", "usersList");

                setTimeout(function () {
                    weakSelf.storage.removeObject(BOCommonConstants.userNameStorageKey);
                    weakSelf.storage.removeObject(UICommonConstants.userTokenStorageKey);
                    weakSelf.appContext.removeObject(BOCommonConstants.userRoleStorageKey);
                    window.location.reload();
                }, 3000);
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

        let formElements: FormType[] = [
            FormTypePasswordProps.initializeWithValidations("Current Password", "", true, [ ValidationMinLengthRule.initialize("Password is too short.", "Invalid password.", 3) ]),
            FormTypePasswordProps.initializeWithValidations("Password", "", true, [ ValidationMinLengthRule.initialize("Password is too short.", "Invalid password.", 3) ]),
            FormTypePasswordProps.initializeWithValidations("Password (Repeat)", "", true, [ ValidationMinLengthRule.initialize("Password is too short.", "Invalid password.", 3) ])
        ];

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
