import { BOController, BreadcrumbNavigationModel, FormType, FormTypeTextProps, FormView } from "iobootstrap-bo-base";
import { AppCryptography, AppServiceHeaderAuthenticationInterceptor, BaseResponseModel, CalloutTypes, DIHooks, ValidationMinLengthRule } from "iobootstrap-ui-base";
import React from "react";
import UpdateUserRequestModel from "../models/UpdateUserRequestModel";
import UserResetPasswordRequestModel from "../models/UserResetPasswordRequestModel";
import UserLoginInformationView from "../views/UserLoginInformationView";

class UserResetPasswordController extends BOController<{}, {}> {

    private appServiceHeaderInterceptor: AppServiceHeaderAuthenticationInterceptor;
    private _updateRequest: UpdateUserRequestModel;

    constructor(props: {}) {
        super(props);

        this.appServiceHeaderInterceptor = DIHooks.Instance.singletonForKey("appServiceHeaderInterceptor");
        this._updateRequest = this.appContext.objectForKey("usersResetPasswordRequest") as UpdateUserRequestModel;
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
        let password: string = values[0];
        this.indicatorPresenter.present();

        const weakSelf = this;
        this.resetPassword(password)
            .catch(() => {
                weakSelf.handleServiceError("", "Cryptography error.");
            });
    }

    private async resetPassword(password: string): Promise<any> {
        const symmetricKeys = await AppCryptography.Instance.getSymmetricKeys();
        this.appServiceHeaderInterceptor.setSymmetricKeys(symmetricKeys.symmetricKey, symmetricKeys.symmetricIV);

        const encryptedNewPassword = await AppCryptography.Instance.encrypt(password);

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_USER_CONTROLLER_NAME}/ResetPassword`;
        const request = new UserResetPasswordRequestModel();
        request.userName = this._updateRequest.userName;
        request.newPassword = encryptedNewPassword;

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
            BreadcrumbNavigationModel.initialize("userResetPassword", "Reset Password")
        ];

        const randomPassword = AppCryptography.Instance.random(8);
        const formElements: FormType[] = [
            FormTypeTextProps.initializeWithValidations("Password", randomPassword, false, [ ValidationMinLengthRule.initialize("Password is too short.", "Invalid password.", 3) ]),
        ];

        return (
            <React.StrictMode>
                <div className="form-wrapper">
                    <FormView navigation={navigation} 
                        resourceHome="Home"
                        title="Reset password"
                        submitButtonName="Save"
                        errorHandler={this.handleFormError}
                        successHandler={this.handleFormSuccess}
                        formElements={formElements} />
                </div>

                <div className="editor-wrapper">
                    <div className="content-wrapper">
                        <UserLoginInformationView userName={this._updateRequest.userName}
                            randomPassword={randomPassword} />
                    </div>
                </div>
            </React.StrictMode>
        );
    }
}

export default UserResetPasswordController;
