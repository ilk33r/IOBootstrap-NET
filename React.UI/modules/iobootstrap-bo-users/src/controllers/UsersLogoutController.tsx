import { AppCryptography, AppServiceHeaderAuthenticationInterceptor, BaseResponseModel, DIHooks, UICommonConstants } from "iobootstrap-ui-base";
import React from "react";
import { BOCommonConstants, BOController, BreadcrumbNavigationModel, QuestionView } from "iobootstrap-bo-base";
import IOLogoutRequestModel from "../models/IOLogoutRequestModel";

class UsersLogoutController extends BOController<{}, {}> {

    private appServiceHeaderInterceptor: AppServiceHeaderAuthenticationInterceptor;
    
    constructor(props: {}) {
        super(props);

        this.appServiceHeaderInterceptor = DIHooks.Instance.singletonForKey("appServiceHeaderInterceptor");
        
        this.handleFormError = this.handleFormError.bind(this);
        this.handleFormSuccess = this.handleFormSuccess.bind(this);
    }

    handleFormError() {
        this.navigateToPage("usersList");
    }

    handleFormSuccess() {
        this.indicatorPresenter.present();
        
        const userName = this.storage.stringForKey(BOCommonConstants.userNameStorageKey) ?? "";
        this.updateSymmetricKeys(userName);
    }

    private updateSymmetricKeys(userName: string) {
        const weakSelf = this;

        AppCryptography.Instance.getSymmetricKeys()
        .then((symmetricKeys) => {
            weakSelf.appServiceHeaderInterceptor.setSymmetricKeys(symmetricKeys.symmetricKey, symmetricKeys.symmetricIV);
            weakSelf.encryptUserName(userName);
        })
        .catch(() => {
            weakSelf.handleServiceError("", "Cryptography error.");
        });
    }

    private encryptUserName(userName: string) {
        const weakSelf = this;

        AppCryptography.Instance.encrypt(userName)
        .then((encryptedData) => {
            weakSelf.logout(encryptedData);
        })
        .catch(() => {
            weakSelf.handleServiceError("", "Cryptography error.");
        });
    }

    private logout(userName: string) {
        const requestPath = `${process.env.REACT_APP_BACKOFFICE_USER_CONTROLLER_NAME}/Logout`;
        const request = new IOLogoutRequestModel();
        request.userName = userName;

        const weakSelf = this;
        this.service.post(requestPath, request, function (response: BaseResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                weakSelf.storage.removeObject(BOCommonConstants.userNameStorageKey);
                weakSelf.storage.removeObject(UICommonConstants.userTokenStorageKey);
                weakSelf.appContext.removeObject(BOCommonConstants.userRoleStorageKey);
                window.location.reload();
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    render() {
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("usersList", "Users"),
            BreadcrumbNavigationModel.initialize("usersLogout", "Sign Out")
        ];

        return (
            <React.StrictMode>
                <QuestionView navigation={navigation} 
                    resourceHome="Home"
                    title="Sign Out"
                    questionMessage="Are you sure want to sign out ?"
                    errorHandler={this.handleFormError}
                    successHandler={this.handleFormSuccess}
                     />
            </React.StrictMode>
        );
    }
}

export default UsersLogoutController;
