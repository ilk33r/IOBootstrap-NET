import React from "react";
import UpdateUserRequestModel from "../models/UpdateUserRequestModel";
import UserChangePasswordRequestModel from "../models/UserChangePasswordRequestModel";
import { AppCryptography, AppServiceHeaderAuthenticationInterceptor, BaseResponseModel, CalloutTypes, DIHooks, UICommonConstants, ValidationMinLengthRule, ValidationRegexRule } from "iobootstrap-ui-base";
import { BOCommonConstants, BOController, BreadcrumbNavigationModel, FormType, FormTypePasswordProps, FormView } from "iobootstrap-bo-base";
import UserChangePasswordState from "../props/UserChangePasswordState";

class UserChangePasswordController extends BOController<{}, UserChangePasswordState> {

    private appServiceHeaderInterceptor: AppServiceHeaderAuthenticationInterceptor;
    private _updateRequest: UpdateUserRequestModel;

    constructor(props: {}) {
        super(props);

        this.state = new UserChangePasswordState();
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

        const requestPath = `${import.meta.env.VITE_BACKOFFICE_USER_CONTROLLER_NAME}/ChangePassword`;
        const request = new UserChangePasswordRequestModel();
        request.oldPassword = encryptedCurrentPassword;
        request.newPassword = encryptedNewPassword;

        const weakSelf = this;
        this.setState({passwordUpdated: true});
        this.service.post(requestPath, request, function (response: BaseResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                weakSelf.showCalloutAndRedirectToHash("User password has been changed successfully.", "usersList");

                setTimeout(function () {
                    weakSelf.storage.removeObject(BOCommonConstants.userNameStorageKey);
                    weakSelf.storage.removeObject(UICommonConstants.userTokenStorageKey);
                    weakSelf.appContext.removeObject(BOCommonConstants.userRoleStorageKey);
                    window.location.reload();
                }, 3000);
            } else {
                weakSelf.setState({passwordUpdated: false});
            }
        }, function (error: string) {
            weakSelf.setState({passwordUpdated: false});
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
            FormTypePasswordProps.initializeWithValidations("Current Password", "", !this.state.passwordUpdated, [ 
                ValidationMinLengthRule.initialize("Password is too short.", "Invalid password.", 3) ]
            ),
            FormTypePasswordProps.initializeWithValidations("Password", "", !this.state.passwordUpdated, [ 
                ValidationMinLengthRule.initialize("Password is too short.", "Invalid password.", 7),
                ValidationRegexRule.initialize("Password must be at least 8 characters and contains special characters.", "Invalid password", /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$/g)
            ]),
            FormTypePasswordProps.initializeWithValidations("Password (Repeat)", "", !this.state.passwordUpdated, [ 
                ValidationMinLengthRule.initialize("Password is too short.", "Invalid password.", 7),
                ValidationRegexRule.initialize("Password must be at least 8 characters and contains special characters.", "Invalid password", /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$/g)
            ])
        ];

        return (
            <React.StrictMode>
                <FormView navigation={navigation} 
                    resourceHome="Home"
                    title="Change password"
                    submitButtonName={!this.state.passwordUpdated ? "Save" : ""}
                    errorHandler={this.handleFormError}
                    successHandler={this.handleFormSuccess}
                    formElements={formElements} />
            </React.StrictMode>
        );
    }
}

export default UserChangePasswordController;
