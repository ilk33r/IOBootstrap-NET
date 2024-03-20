import React from "react";
import UpdateUserRequestModel from "../models/UpdateUserRequestModel";
import { BaseResponseModel, CalloutTypes, Controller, ValidationMinLengthRule } from "iobootstrap-ui-base";
import { BreadcrumbNavigationModel, FormDataOptionModel, FormType, FormTypeSelectProps, FormTypeTextProps, FormView } from "iobootstrap-bo-base";
import { UserRoles } from "iobootstrap-bo-base";

class UsersUpdateController extends Controller<{}, {}> {

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

        const formElements: FormType[] = [
            FormTypeTextProps.initializeWithValidations("User Name", this._updateRequest.userName, true, [ ValidationMinLengthRule.initialize("User name is too short.", "Invalid user name.", 3) ]),
            FormTypeSelectProps.initialize("Role", this._updateRequest.userRole.toString(), true, [
                FormDataOptionModel.initialize(UserRoles.getRoleName(UserRoles.SuperAdmin), UserRoles.SuperAdmin.toString()),
                FormDataOptionModel.initialize(UserRoles.getRoleName(UserRoles.Admin), UserRoles.Admin.toString()),
                FormDataOptionModel.initialize(UserRoles.getRoleName(UserRoles.User), UserRoles.User.toString()),
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
}

export default UsersUpdateController;
