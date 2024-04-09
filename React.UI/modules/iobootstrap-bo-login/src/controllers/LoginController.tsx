import { AppCryptography, AppServiceHeaderAuthenticationInterceptor, BaseResponseModel, DIHooks, UICommonConstants } from 'iobootstrap-ui-base';
import AuthenticationRequestModel from '../models/AuthenticationRequestModel';
import AuthenticationResponseModel from '../models/AuthenticationResponseModel';
import LoginProps from '../props/LoginProps';
import LoginState from '../props/LoginState';
import React from 'react';
import { BOCommonConstants, BOController } from 'iobootstrap-bo-base';

class LoginController extends BOController<LoginProps, LoginState> {

    private appServiceHeaderInterceptor: AppServiceHeaderAuthenticationInterceptor;
    
    constructor(props: LoginProps) {
        super(props);

        this.state = new LoginState();
        this.appServiceHeaderInterceptor = DIHooks.Instance.singletonForKey("appServiceHeaderInterceptor");

        this.handleUserNameChange = this.handleUserNameChange.bind(this);
        this.handlePasswordChange = this.handlePasswordChange.bind(this);
        this.handleLogin = this.handleLogin.bind(this);
    }

    public handleUserNameChange(event: React.ChangeEvent<HTMLInputElement>) {
        const newState = new LoginState();
        newState.userName = event.target.value;
        newState.password = this.state.password;
        newState.errorMessage = this.state.errorMessage;

        this.setState(newState);
    }

    public handlePasswordChange(event: React.ChangeEvent<HTMLInputElement>) {
        const newState = new LoginState();
        newState.userName = this.state.userName;
        newState.password = event.target.value;
        newState.errorMessage = this.state.errorMessage;

        this.setState(newState);
    }

    private loginSuccessHandler() {
        this.props.loginSuccessHandler();
    }

    public handleServiceError(title: string, message: string) {
        super.handleServiceError(title, message);

        const newState = new LoginState();
        newState.userName = this.state.userName;
        newState.password = this.state.password;
        newState.errorMessage = message;
        this.setState(newState);
    }

    public handleInvalidCredential(response: BaseResponseModel) {
        this.handleServiceError(response.status?.message ?? "", response.status?.detailedMessage ?? "");
    }

    public handleLogin(event: React.FormEvent<HTMLFormElement>) {
        event.preventDefault();

        this.indicatorPresenter.present();

        const weakSelf = this;
        AppCryptography.Instance.encrypt(this.state.password)
          .then((encryptedData) => {
            weakSelf.appServiceHeaderInterceptor.setSymmetricKeys(
              encryptedData.symmetricKey,
              encryptedData.symmetricIV
            );
            weakSelf.authenticate(encryptedData.encrypted);
          })
          .catch(() => {
            weakSelf.indicatorPresenter.dismiss();

            const newState = new LoginState();
            newState.userName = weakSelf.state.userName;
            newState.password = weakSelf.state.password;
            newState.errorMessage = "Encryption error.";
            weakSelf.setState(newState);
          });
    }

    private authenticate(encryptedPassword: string) {
        const request = new AuthenticationRequestModel();
        request.UserName = this.state.userName;
        request.Password = encryptedPassword;

        const requestURL = `${this.props.controllerName}/Authenticate`;
        const weakSelf = this;
        this.service.post(requestURL, request, function (response: AuthenticationResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                const token = (response.token == null) ? "" : response.token;
                weakSelf.storage.setStringForKey(UICommonConstants.userTokenStorageKey, token);

                if (response.userRole != null) {
                    weakSelf.appContext.setNumberForKey(BOCommonConstants.userRoleStorageKey, response.userRole);
                }

                weakSelf.decryptResponseAndUpdateState(response.userName ?? "");
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    private decryptResponseAndUpdateState(encryptedUserName: string) {
        const weakSelf = this;

        AppCryptography.Instance.decrypt(encryptedUserName)
          .then((decrypted) => {
            weakSelf.storage.setStringForKey(
              BOCommonConstants.userNameStorageKey,
              decrypted
            );
            weakSelf.loginSuccessHandler();
          })
          .catch(() => {
            weakSelf.loginSuccessHandler();
          });
    }

    public render() {
        const formGroupErrorClass = (this.state.errorMessage.length > 0) ? "form-group has-error" : "form-group";
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
                                                    <input type="password" className="form-control" id="inputPassword3" placeholder="Password" value={this.state.password} onChange={this.handlePasswordChange} />
                                                </div>
                                            </div>
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
