import { Controller, UICommonConstants } from "iobootstrap-ui-base";
import React from "react";
import { BOCommonConstants, BreadcrumbNavigationModel, QuestionView } from "iobootstrap-bo-base";

class UsersLogoutController extends Controller<{}, {}> {

    constructor(props: {}) {
        super(props);

        this.handleFormError = this.handleFormError.bind(this);
        this.handleFormSuccess = this.handleFormSuccess.bind(this);
    }

    handleFormError() {
        this.navigateToPage("usersList");
    }

    handleFormSuccess() {
        this.storage.removeObject(BOCommonConstants.userNameStorageKey);
        this.storage.removeObject(UICommonConstants.userTokenStorageKey);
        this.appContext.removeObject(BOCommonConstants.userRoleStorageKey);
        window.location.reload();
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
