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
        const formGroupErrorClass = (this.state.errorMessage.length > 0) ? "form-group has-error" : "form-group";
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
                    <div className={formGroupErrorClass}>
                        <label htmlFor="inputCaptcha" className="col-sm-2 control-label">Captcha</label>
                        <div className="col-sm-10">
                            <input type="text" className="form-control" id="inputCaptcha" placeholder="Captcha" onChange={this.handleCaptchaChange} />
                            <img src={captchaURL} alt="Captcha" />
                        </div>
                    </div>
                </React.StrictMode>
            );
        }

        return (
            <React.StrictMode>
                <div className="content-wrapper">
                    <section className="content">
                        <div className="row">
                            <div className="col-md-6">
                                <div className="box box-info">
                                    <div className="box-header with-border">
                                        <h3 className="box-title">{process.env.REACT_APP_APP_NAME} Backoffice</h3>
                                    </div>
                                    <form className="form-horizontal" id="loginForm" onSubmit={this.handleLogin}>
                                        <div className="box-body">
                                            <div className={formGroupErrorClass}>
                                                <label htmlFor="inputEmail3" className="col-sm-2 control-label">User Name</label>
                                                <div className="col-sm-10">
                                                    <input type="text" className="form-control" id="inputEmail3" placeholder="User Name" value={this.state.userName} onChange={this.handleUserNameChange} />
                                                </div>
                                            </div>
                                            <div className={formGroupErrorClass}>
                                                <label htmlFor="inputPassword3" className="col-sm-2 control-label">Password</label>
                                                <div className="col-sm-10">
                                                    <input type="password" className="form-control" id="inputPassword3" placeholder="Password" value={this.state.password ?? ""} onChange={this.handlePasswordChange} />
                                                </div>
                                            </div>
                                            {captchaComponent}
                                            <div className={formGroupErrorClass}>
                                                <div className="col-sm-10">
                                                    <span className="help-block">{this.state.errorMessage}</span>
                                                </div>
                                            </div>
                                        </div>
                                        <div className="box-footer">
                                            <button type="submit" className="btn btn-info pull-right">Sign in</button>
                                        </div>
                                    </form>
                                </div>
                            </div>
                        </div>
                    </section>
                </div>
            </React.StrictMode>
          );
    }
}

export default LoginController;
