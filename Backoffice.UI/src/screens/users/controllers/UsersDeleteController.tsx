import BaseResponseModel from "../../../common/models/BaseResponseModel";
import BreadcrumbNavigationModel from "../../shared/models/BreadcrumbNavigationModel";
import Controller from "../../../presentation/controllers/Controller";
import DeleteUserRequestModel from "../models/DeleteUserRequestModel";
import QuestionView from "../../shared/views/QuestionView";
import React from "react";

class UsersDeleteController extends Controller<{}, {}> {

    private _deleteRequest: DeleteUserRequestModel;

    constructor(props: {}) {
        super(props);

        this._deleteRequest = this.appContext.objectForKey("usersDeleteRequest") as DeleteUserRequestModel;
        if (this._deleteRequest == null) {
            this.navigateToPage("usersList");
            return;
        }

        this.handleFormError = this.handleFormError.bind(this);
        this.handleFormSuccess = this.handleFormSuccess.bind(this);
    }

    handleFormError() {
        this.navigateToPage("usersList");
    }

    handleFormSuccess() {
        this.indicatorPresenter.present();

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_USER_CONTROLLER_NAME}/DeleteUser`;
        const request = new DeleteUserRequestModel();
        request.userId = this._deleteRequest.userId;

        const weakSelf = this;
        this.service.post(requestPath, request, function (response: BaseResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                weakSelf.showCalloutAndRedirectToHash("User has been deleted successfully.", "usersList");
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    render() {
        if (this._deleteRequest == null) {
            return (<React.StrictMode></React.StrictMode>);
        }
    
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("usersList", "Users"),
            BreadcrumbNavigationModel.initialize("usersDelete", "Delete User")
        ];

        return (
            <React.StrictMode>
                <QuestionView navigation={navigation} 
                    resourceHome="Home"
                    title="Delete a user"
                    questionMessage="Are you sure want to delete this user ?"
                    errorHandler={this.handleFormError}
                    successHandler={this.handleFormSuccess}
                     />
            </React.StrictMode>
        );
    }
}

export default UsersDeleteController;
