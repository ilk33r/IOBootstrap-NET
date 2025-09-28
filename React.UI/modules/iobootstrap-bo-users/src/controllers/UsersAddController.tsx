import AddUserRequestModel from "../models/AddUserRequestModel";
import React from "react";
import { AppCryptography, AppServiceHeaderAuthenticationInterceptor, BaseResponseModel, CalloutTypes, DIHooks, ValidationBackofficeRequestRule, ValidationDateRule, ValidationMinLengthRule, ValidationRequiredRule } from "iobootstrap-ui-base";
import { BOController, BreadcrumbNavigationModel, FormDataOptionModel, FormType, FormTypeDateProps, FormTypeSelectProps, FormTypeTextProps, FormView } from "iobootstrap-bo-base";
import UserLoginInformationView from "../views/UserLoginInformationView";
import UserAddState from "../props/UserAddState";

class UsersAddController extends BOController<{}, UserAddState> {

    private appServiceHeaderInterceptor: AppServiceHeaderAuthenticationInterceptor;
    private temporaryPassword: string;

    constructor(props: {}) {
        super(props);

        this.state = new UserAddState();
        this.appServiceHeaderInterceptor = DIHooks.Instance.singletonForKey("appServiceHeaderInterceptor");
        this.temporaryPassword = AppCryptography.Instance.random(8);
        
        this.handleFormError = this.handleFormError.bind(this);
        this.handleFormSuccess = this.handleFormSuccess.bind(this);
    }

    handleFormError(errorTitle: string, errorMessage: string) {
        this.calloutPresenter.show(CalloutTypes.danger, errorTitle, errorMessage);
    }

    handleFormSuccess(values: string[], blobs: Blob[]) {
        this.indicatorPresenter.present();

        const weakSelf = this;
        const isActive = (values[3] == "yes") ? true : false;
        this.addUser(values[0], values[1], Number(values[2]), isActive, values[4])
            .catch(() => {
                weakSelf.handleServiceError("", "Cryptography error.");
            });
    }

    private async addUser(userName: string, password: string, userRole: number, isActive: boolean, activationEndDate: string): Promise<any> {
        const symmetricKeys = await AppCryptography.Instance.getSymmetricKeys();
        this.appServiceHeaderInterceptor.setSymmetricKeys(symmetricKeys.symmetricKey, symmetricKeys.symmetricIV);

        const encryptPasswords = await AppCryptography.Instance.encrypt(password);

        const requestPath = `${import.meta.env.VITEBACKOFFICE_USER_CONTROLLER_NAME}/AddUser`;
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

        const weakSelf = this;
        const formElements: FormType[] = [
            FormTypeTextProps.initializeWithChangeListener("User Name", "", true, [ 
                ValidationMinLengthRule.initialize("User name is too short.", "Invalid user name.", 3),
                ValidationBackofficeRequestRule.initialize("Invalid characters.", "Invalid characters.")
            ], (index, text) => {
                const newState = new UserAddState();
                newState.userName = text;

                weakSelf.setState(newState);
            }),
            FormTypeTextProps.initializeWithValidations("Password", this.temporaryPassword, false, [ ValidationMinLengthRule.initialize("Password is too short.", "Invalid password.", 3) ]),
            FormTypeSelectProps.initialize("Role", "2", true, userRoleFormDataOptions),
            FormTypeSelectProps.initialize("Active", "", true, [ 
                FormDataOptionModel.initialize("NO", "no"),
                FormDataOptionModel.initialize("YES", "yes")
            ]),
            FormTypeDateProps.initializeWithValidations("End Date", "", true, [ 
                ValidationRequiredRule.initialize("End date is required.", "Invalid end date."),
                ValidationDateRule.initialize("Invalid date.", "Invalid date.")
            ])
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
                    
                <UserLoginInformationView userName={this.state.userName.RemoveHTML()}
                    temporaryPassword={this.temporaryPassword} />
            </React.StrictMode>
        );
    }
}

export default UsersAddController;
