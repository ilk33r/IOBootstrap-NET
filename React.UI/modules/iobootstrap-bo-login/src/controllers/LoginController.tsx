import { AppCryptography, BaseResponseModel, UICommonConstants } from 'iobootstrap-ui-base';
import AuthenticationRequestModel from '../models/AuthenticationRequestModel';
import AuthenticationResponseModel from '../models/AuthenticationResponseModel';
import LoginProps from '../props/LoginProps';
import LoginState from '../props/LoginState';
import React from 'react';
import { BOCommonConstants, BOController } from 'iobootstrap-bo-base';

class LoginController extends BOController<LoginProps, LoginState> {
    
    constructor(props: LoginProps) {
        super(props);

        this.state = new LoginState();

        this.handleUserNameChange = this.handleUserNameChange.bind(this);
        this.handlePasswordChange = this.handlePasswordChange.bind(this);
        this.handleCaptchaChange = this.handleCaptchaChange.bind(this);
        this.handleLogin = this.handleLogin.bind(this);
    }

    private handleUserNameChange(event: React.ChangeEvent<HTMLInputElement>) {
        this.setState({
            userName: event.target.value
        });
    }

    private handlePasswordChange(event: React.ChangeEvent<HTMLInputElement>) {
        this.setState({
            password: event.target.value
        });
    }

    private handleCaptchaChange(event: React.ChangeEvent<HTMLInputElement>) {
        this.setState({
            captcha: event.target.value
        });
    }

    private loginSuccessHandler() {
        this.props.loginSuccessHandler();
    }

    public handleServiceError(title: string, message: string) {
        super.handleServiceError(title, message);

        this.setState({
            userName: "",
            password: "",
            captcha: "",
            errorMessage: message
        });
    }

    public handleInvalidCredential(response: BaseResponseModel) {
        this.handleServiceError(response.status?.message ?? "", response.status?.detailedMessage ?? "");
    }

    public handleCapthca(response: BaseResponseModel) {
        this.setState({
            password: "",
            captcha: "",
            captchaID: (response.status?.detailedMessage === undefined || response.status?.detailedMessage === "") ? null : response.status?.detailedMessage
        });
    }

    public handleLogin(event: React.FormEvent<HTMLFormElement>) {
        event.preventDefault();

        if (this.state.password === null || this.state.password.length < 4) {
            return;
        }

        this.indicatorPresenter.present();

        const weakSelf = this;
        this.authenticate()
            .catch(() => {
                weakSelf.indicatorPresenter.dismiss();
                weakSelf.setState({
                    password: "",
                    captcha: "",
                    errorMessage: "Encryption error."
                });
            });
    }

    private async authenticate(): Promise<any> {
        const encryptedPassword = await AppCryptography.Instance.encrypt(this.state.password ?? "");
        let encryptedCaptcha: string | null = null;
        if (this.state.captcha != null) {
            encryptedCaptcha = await AppCryptography.Instance.encrypt(this.state.captcha);
        }

        const request = new AuthenticationRequestModel();
        request.UserName = this.state.userName;
        request.Password = encryptedPassword;
        request.CaptchaID = this.state.captchaID;
        request.EncryptedCaptcha = encryptedCaptcha;

        const requestURL = `${this.props.controllerName}/Authenticate`;
        const weakSelf = this;
        this.service.post(requestURL, request, function (response: AuthenticationResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                if (response.userRole != null) {
                    weakSelf.appContext.setNumberForKey(BOCommonConstants.userRoleStorageKey, response.userRole);
                }

                weakSelf.decryptTokenAndUserName(response.token, response.userName ?? "")
                            .then(() => {
                                weakSelf.loginSuccessHandler();
                            })
                            .catch(() => {
                                weakSelf.loginSuccessHandler();
                            });
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    private async decryptTokenAndUserName(encryptedToken: string | null, encryptedUserName: string): Promise<any> {
        const cookieAuthentication = process.env.REACT_APP_COOKIE_AUTHENTICATION;
        if (cookieAuthentication !== "true") {
            this.storage.setStringForKey(UICommonConstants.userTokenStorageKey, encryptedToken ?? "");
        }

        const decryptedUserName = await AppCryptography.Instance.decrypt(encryptedUserName);
        this.storage.setStringForKey(BOCommonConstants.userNameStorageKey, decryptedUserName);
    }

    public render() {
        const formControlClass = (this.state.errorMessage.length > 0) ? "form-control is-invalid" : "form-control";
        let captchaComponent: React.JSX.Element;

        if (this.state.captchaID == null) {
            captchaComponent = (
                <React.StrictMode>
                </React.StrictMode>
            );
        } else {
            const captchaURL = `${process.env.REACT_APP_API_URL}/ImageAsset/GetCaptcha?id=${this.state.captchaID ?? ""}`;
            captchaComponent = (
                <React.StrictMode>
                    <div className="mb-4">
                        <img src={captchaURL} alt="Captcha" />
                    </div>
                    <div className="mb-4">
                        <div className="form-floating">
                            <input type="text" id="inputCaptcha" className={formControlClass} placeholder="Captcha" onChange={this.handleCaptchaChange} />
                            <label htmlFor="inputCaptcha">Captcha</label>
                        </div>
                    </div>
                </React.StrictMode>
            );
        }

        return (
            <React.StrictMode>
                <section className="container-fluid">
                    <div className="row mb-5 mt-5">
                        <div className="col-sm-12 col-md-6 mx-auto">
                            <div className="box">
                                <div className="box-header">
                                    <h3>{process.env.REACT_APP_APP_NAME} Backoffice</h3>
                                </div>
                                <form className="needs-validation" id="loginForm" onSubmit={this.handleLogin}>
                                    <div className="box-body">
                                        <div className="mb-4">
                                            <div className="form-floating">
                                                <input type="text" id="inputUserName" className={formControlClass} placeholder="User Name" value={this.state.userName} onChange={this.handleUserNameChange} />
                                                <label htmlFor="inputUserName">User Name</label>
                                            </div>
                                        </div>
                                        <div className="mb-4">
                                            <div className="form-floating">
                                                <input type="password" id="inputPassword" className={formControlClass} placeholder="Password" value={this.state.password ?? ""} onChange={this.handlePasswordChange} />
                                                <span className="invalid-feedback">{this.state.errorMessage}</span>
                                                <label htmlFor="inputPassword">Password</label>
                                            </div>
                                        </div>
                                        {captchaComponent}
                                    </div>
                                    <div className="box-footer text-end">
                                        <button type="submit" className="btn btn-primary btn-lg">Sign in</button>
                                    </div>
                                </form>
                            </div>
                        </div>
                    </div>
                    <div className="row mb-5"></div>
                    <div className="row mb-5"></div>
                    <div className="row mb-5"></div>
                </section>
            </React.StrictMode>
          );
    }
}

export default LoginController;
