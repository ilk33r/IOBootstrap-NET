import BreadcrumbNavigationModel from "../../shared/models/BreadcrumbNavigationModel";
import Controller from "../../../presentation/controllers/Controller";
import QuestionView from "../../shared/views/QuestionView";
import React from "react";
import CommonConstants from "../../../common/constants/CommonConstants";

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
        this.storage.removeObject(CommonConstants.userNameStorageKey);
        this.storage.removeObject(CommonConstants.userTokenStorageKey);
        this.appContext.removeObject(CommonConstants.userRoleStorageKey);
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
