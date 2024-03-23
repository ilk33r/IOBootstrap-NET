import AddUserRequestModel from "../models/AddUserRequestModel";
import React from "react";
import { BaseResponseModel, CalloutTypes, Controller, DIHooks, ValidationMinLengthRule } from "iobootstrap-ui-base";
import { BreadcrumbNavigationModel, FormDataOptionModel, FormType, FormTypePasswordProps, FormTypeSelectProps, FormTypeTextProps, FormView } from "iobootstrap-bo-base";

class UsersAddController extends Controller<{}, {}> {

    constructor(props: {}) {
        super(props);

        this.handleFormError = this.handleFormError.bind(this);
        this.handleFormSuccess = this.handleFormSuccess.bind(this);
    }

    handleFormError(errorTitle: string, errorMessage: string) {
        this.calloutPresenter.show(CalloutTypes.danger, errorTitle, errorMessage);
    }

    handleFormSuccess(values: string[], blobs: Blob[]) {
        if (values[1] !== values[2]) {
            this.handleFormError("Passwords did not match.", "Invalid password.");
            return;
        }

        this.indicatorPresenter.present();
        
        const requestPath = `${process.env.REACT_APP_BACKOFFICE_USER_CONTROLLER_NAME}/AddUser`;
        const request = new AddUserRequestModel();
        request.userName = values[0];
        request.password = values[1];
        request.userRole = Number(values[3]);

        const weakSelf = this;
        this.service.post(requestPath, request, function (response: BaseResponseModel) {
            if (response.status !== undefined && response.status.code === 700) {
                const helpText = 'User ' + request.userName + ' is exists.';
                weakSelf.indicatorPresenter.dismiss();
                weakSelf.calloutPresenter.show(CalloutTypes.danger, "Invalid username.", helpText);
                return;
            }

            if (weakSelf.handleServiceSuccess(response)) {
                weakSelf.showCalloutAndRedirectToHash("User has been added successfully.", "usersList");
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    render() {
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("usersList", "Users"),
            BreadcrumbNavigationModel.initialize("usersAdd", "Add User")
        ];

        let userRoleFormDataOptions: FormDataOptionModel[] = []
        const userRoleFormDataOptionsHook = DIHooks.Instance.hookForKey("userRoleFormDataOptions")
        if (userRoleFormDataOptionsHook != null) {
            const userRoleFormDataOptionsAny = userRoleFormDataOptionsHook(null);
            if (userRoleFormDataOptionsAny != null) {
                userRoleFormDataOptions = userRoleFormDataOptionsAny;
            }
        }

        const formElements: FormType[] = [
            FormTypeTextProps.initializeWithValidations("User Name", "", true, [ ValidationMinLengthRule.initialize("User name is too short.", "Invalid user name.", 3) ]),
            FormTypePasswordProps.initializeWithValidations("Password", "", true, [ ValidationMinLengthRule.initialize("Password is too short.", "Invalid password.", 3) ]),
            FormTypePasswordProps.initializeWithValidations("Password (Repeat)", "", true, [ ValidationMinLengthRule.initialize("Password is too short.", "Invalid password.", 3) ]),
            FormTypeSelectProps.initialize("Role", "", true, userRoleFormDataOptions)
        ];

        return (
            <React.StrictMode>
                <FormView navigation={navigation} 
                    resourceHome="Home"
                    title="Add a new user"
                    submitButtonName="Add"
                    errorHandler={this.handleFormError}
                    successHandler={this.handleFormSuccess}
                    formElements={formElements} />
            </React.StrictMode>
        );
    }
}

export default UsersAddController;
