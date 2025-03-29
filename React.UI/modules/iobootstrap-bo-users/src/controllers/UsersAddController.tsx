import AddUserRequestModel from "../models/AddUserRequestModel";
import React from "react";
import { AppCryptography, AppServiceHeaderAuthenticationInterceptor, BaseResponseModel, CalloutTypes, DIHooks, ValidationMinLengthRule, ValidationRequiredRule } from "iobootstrap-ui-base";
import { BOController, BreadcrumbNavigationModel, FormDataOptionModel, FormType, FormTypeDateProps, FormTypePasswordProps, FormTypeSelectProps, FormTypeTextProps, FormView } from "iobootstrap-bo-base";

class UsersAddController extends BOController<{}, {}> {

    private appServiceHeaderInterceptor: AppServiceHeaderAuthenticationInterceptor;

    constructor(props: {}) {
        super(props);

        this.appServiceHeaderInterceptor = DIHooks.Instance.singletonForKey("appServiceHeaderInterceptor");
        
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

        const weakSelf = this;
        this.addUser(values[0], values[1], Number(values[3]), Boolean(values[4]), values[5])
            .catch(() => {
                weakSelf.handleServiceError("", "Cryptography error.");
            });
    }

    private async addUser(userName: string, password: string, userRole: number, isActive: boolean, activationEndDate: string): Promise<any> {
        const symmetricKeys = await AppCryptography.Instance.getSymmetricKeys();
        this.appServiceHeaderInterceptor.setSymmetricKeys(symmetricKeys.symmetricKey, symmetricKeys.symmetricIV);

        const encryptPasswords = await AppCryptography.Instance.encrypt(password);

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_USER_CONTROLLER_NAME}/AddUser`;
        const request = new AddUserRequestModel();
        request.userName = userName;
        request.password = encryptPasswords;
        request.userRole = userRole;
        request.isActive = isActive;
        request.activationEndDate = activationEndDate;

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
            FormTypeSelectProps.initialize("Role", "", true, userRoleFormDataOptions),
            FormTypeSelectProps.initialize("Active", "", true, [ 
                FormDataOptionModel.initialize("NO", "0"),
                FormDataOptionModel.initialize("YES", "1")
            ]),
            FormTypeDateProps.initializeWithValidations("End Date", "", true, [ ValidationRequiredRule.initialize("End date is required.", "Invalid end date.") ])
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
