import { Controller, UICommonConstants } from 'iobootstrap-ui-base';
import AuthenticationRequestModel from '../models/AuthenticationRequestModel';
import AuthenticationResponseModel from '../models/AuthenticationResponseModel';
import LoginProps from '../props/LoginProps';
import LoginState from '../props/LoginState';
import React from 'react';
import { BOCommonConstants } from 'iobootstrap-bo-base';

class LoginController extends Controller<LoginProps, LoginState> {

    constructor(props: LoginProps) {
        super(props);

        this.state = new LoginState();

        this.handleUserNameChange = this.handleUserNameChange.bind(this);
        this.handlePasswordChange = this.handlePasswordChange.bind(this);
        this.handleLogin = this.handleLogin.bind(this);
    }

    public handleUserNameChange(event: React.ChangeEvent<HTMLInputElement>) {
        const newState = new LoginState();
        newState.userName = event.target.value;
        newState.password = this.state.password;
        newState.errorMessage = "";

        this.setState(newState);
    }

    public handlePasswordChange(event: React.ChangeEvent<HTMLInputElement>) {
        const newState = new LoginState();
        newState.userName = this.state.userName;
        newState.password = event.target.value;
        newState.errorMessage = "";

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

    public handleLogin(event: React.FormEvent<HTMLFormElement>) {
        event.preventDefault();

        this.indicatorPresenter.present();

        const request = new AuthenticationRequestModel();
        request.UserName = this.state.userName;
        request.Password = this.state.password;

        const requestURL = `${this.props.controllerName}/Authenticate`;
        const weakSelf = this;
        this.service.post(requestURL, request, function (response: AuthenticationResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                const token = (response.token == null) ? "" : response.token;
                const userName = (response.userName == null) ? "" : response.userName;
                weakSelf.storage.setStringForKey(UICommonConstants.userTokenStorageKey, token);
                weakSelf.storage.setStringForKey(BOCommonConstants.userNameStorageKey, userName);

                if (response.userRole != null) {
                    weakSelf.appContext.setNumberForKey(BOCommonConstants.userRoleStorageKey, response.userRole);
                }

                weakSelf.loginSuccessHandler();
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
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
