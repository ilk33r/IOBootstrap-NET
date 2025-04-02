import AddUserRequestModel from "../models/AddUserRequestModel";
import React from "react";
import { AppCryptography, AppServiceHeaderAuthenticationInterceptor, BaseResponseModel, CalloutTypes, DIHooks, ValidationMinLengthRule, ValidationRequiredRule } from "iobootstrap-ui-base";
import { BOController, BreadcrumbNavigationModel, FormDataOptionModel, FormType, FormTypeDateProps, FormTypeSelectProps, FormTypeTextProps, FormView } from "iobootstrap-bo-base";

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

        const randomPassword = AppCryptography.Instance.random(8);
        const formElements: FormType[] = [
            FormTypeTextProps.initializeWithValidations("User Name", "", true, [ ValidationMinLengthRule.initialize("User name is too short.", "Invalid user name.", 3) ]),
            FormTypeTextProps.initializeWithValidations("Password", randomPassword, false, [ ValidationMinLengthRule.initialize("Password is too short.", "Invalid password.", 3) ]),
            FormTypeSelectProps.initialize("Role", "", true, userRoleFormDataOptions),
            FormTypeSelectProps.initialize("Active", "", true, [ 
                FormDataOptionModel.initialize("NO", "no"),
                FormDataOptionModel.initialize("YES", "yes")
            ]),
            FormTypeDateProps.initializeWithValidations("End Date", "", true, [ ValidationRequiredRule.initialize("End date is required.", "Invalid end date.") ])
        ];

        return (
            <React.StrictMode>
                <div className="form-wrapper">
                    <FormView navigation={navigation} 
                        resourceHome="Home"
                        title="Add a new user"
                        submitButtonName="Add"
                        errorHandler={this.handleFormError}
                        successHandler={this.handleFormSuccess}
                        formElements={formElements} />
                </div>
                    
                <div className="editor-wrapper">
                    <div className="content-wrapper">
                        <section className="content">
                            <div className="row">
                                <div className="col-md-12">
                                    <div id="userInfo" className="editor" contentEditable>
                                        <table width="100%" cellPadding={0} cellSpacing={0} border={0} style={{margin: '0px auto', color: '#000000', backgroundColor: 'rgb(244, 244, 244)'}}>
                                            <tbody>
                                                <tr>
                                                    <td align="center">
                                                        <table width="600" cellPadding={0} cellSpacing={0} border={0} style={{margin: '0px auto', maxWidth: '600px', backgroundColor: '#ffffff', border: '4px solid #000000', borderRadius: '16px', overflow: 'hidden', boxShadow: 'rgba(0, 0, 0, 0.1) 0px 4px 8px'}}>
                                                            <tbody>
                                                                <tr>
                                                                    <td style={{backgroundColor: '#11BFDE', color: '#ffffff', textAlign: 'center', paddingTop: '15px', paddingBottom: '15px', fontSize: '24px', fontWeight: 'bold', borderTopLeftRadius: '16px', borderTopRightRadius: '16px'}}>
                                                                        {process.env.REACT_APP_APP_NAME} LOGIN
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style={{padding: '10px', textAlign: 'center'}}>
                                                                        <p style={{textAlign: 'left'}}>Your account has been created.</p>
                                                                        <p style={{textAlign: 'left'}}><b>{process.env.REACT_APP_APP_NAME}</b> login information is below.</p>
                                                                        <table width="100%" cellPadding={0} cellSpacing={0} border={0} style={{margin: '0px auto', border: '2px solid #000000', padding: '15px', borderRadius: '8px', backgroundColor: 'rgb(249, 249, 249)'}}>
                                                                            <tbody>
                                                                                <tr>
                                                                                    <td width="50%" style={{padding: '10px', border: '1px solid #000000'}}>
                                                                                        <b>User Name</b>
                                                                                    </td>
                                                                                    <td width="50%" style={{padding: '10px', border: '1px solid #000000'}}>
                                                                                        <b>NA</b>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td width="50%" style={{padding: '10px', border: '1px solid #000000'}}>
                                                                                        <b>Password</b>
                                                                                    </td>
                                                                                    <td width="50%" style={{padding: '10px', border: '1px solid #000000'}}>
                                                                                        <b>{randomPassword}</b>
                                                                                    </td>
                                                                                </tr>
                                                                            </tbody>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </section>
                    </div>
                </div>
            </React.StrictMode>
        );
    }
}

export default UsersAddController;
