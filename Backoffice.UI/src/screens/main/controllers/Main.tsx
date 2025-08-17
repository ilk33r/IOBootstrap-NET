import CheckTokenRequestModel from '../models/CheckTokenRequestModel';
import CheckTokenResponseModel from '../models/CheckTokenResponseModel';
import MainProps from '../props/MainProps';
import MainState from '../props/MainState';
import NavigationView from '../../shared/views/NavigationView';
import SelectionWrapperView from '../../shared/views/SelectionWrapperView';
import React from 'react';
import { AppCryptography, AppServiceHeaderAuthenticationInterceptor, CalloutPresenter, CalloutViewPresenter, DIHooks, IndicatorPresenter, IndicatorViewPresenter, UICommonConstants, UploadModalPresenter, UploadModalViewPresenter } from 'iobootstrap-ui-base';
import { BOCommonConstants, BOController, FooterView, HeaderView } from 'iobootstrap-bo-base';
import { MenuController } from 'iobootstrap-bo-menu';
import { LoginController } from 'iobootstrap-bo-login';
import HandshakeResponseModel from '../models/HandshakeResponseModel';

class Main extends BOController<MainProps, MainState> {

    private appServiceHeaderInterceptor: AppServiceHeaderAuthenticationInterceptor;

    constructor(props: MainProps) {
        super(props);

        this.state = new MainState();
        this.appServiceHeaderInterceptor = DIHooks.Instance.singletonForKey("appServiceHeaderInterceptor");

        this.service.baseUrl = (process.env.REACT_APP_API_URL === undefined) ? "" : process.env.REACT_APP_API_URL;
        this.service.appServiceHeaderInterceptor = DIHooks.Instance.singletonForKey("appServiceHeaderInterceptor");

        const authorization = (process.env.REACT_APP_AUTHORIZATION === undefined) ? "" : process.env.REACT_APP_AUTHORIZATION;
        this.appServiceHeaderInterceptor.initialize(authorization);
        
        if (props.calloutView !== undefined) {
            const calloutPresenter = this.calloutPresenter as CalloutPresenter;
            calloutPresenter.calloutView = props.calloutView.current as CalloutViewPresenter;
        }

        if (props.indicatorView !== undefined) {
            const indicatorPresenter = this.indicatorPresenter as IndicatorPresenter;
            indicatorPresenter.indicatorView = props.indicatorView.current as IndicatorViewPresenter;
        }

        if (props.uploadModalView !== undefined) {
            const uploadModalPresenter = this.uploadModalPresenter as UploadModalPresenter;
            uploadModalPresenter.uploadModalView = props.uploadModalView.current as UploadModalViewPresenter;
        }

        this.handleLoginSuccess = this.handleLoginSuccess.bind(this);
    }

    private updateLocation(hash: string) {
        let hashName = hash;
        if (hash.startsWith('#!')) {
            hashName = hash.substring(2, 2 + hash.length);

            const newState = new MainState();
            newState.isLoggedIn = this.state.isLoggedIn;
            
            if (hashName.startsWith("selection/")) {
                newState.pageHash = this.state.pageHash;
                newState.selectionHash = hashName;
            } else {
                newState.pageHash = hashName;
                newState.selectionHash = null;
            }

            this.setState(newState);
        }
    }

    public componentDidMount?(): void {
        if (window.location.hash === "#!usersLogout") {
            window.location.hash = "#!dashboard";
        }

        const weakSelf = this;
        $(window).on("hashchange", function(e) {
            weakSelf.updateLocation(e.target.location.hash);
        });

        this.handshake();
    }

    private handshake() {
        this.indicatorPresenter.present();

        const newState = new MainState();
        newState.isLoggedIn = false;

        const requestPath = `${process.env.REACT_APP_HANDSHAKE_CONTROLLER_NAME}/Index`;
        const weakSelf = this;

        this.service.get(requestPath, function (response: HandshakeResponseModel) {
            weakSelf.indicatorPresenter.dismiss();

            if (response.status?.code !== 200) {
                weakSelf.setState(newState);
                return;
            }

            if (response.keyID == null || response.publicKeyExponent == null || response.publicKeyModulus == null) {
                weakSelf.setState(newState);
                return;
            }

            weakSelf.appServiceHeaderInterceptor.setKeyID(response.keyID);
            AppCryptography.Instance.initialize(
              response.publicKeyExponent,
              response.publicKeyModulus
            )
              .then(() => {
                weakSelf.createSymmetricKeysAndCheckToken();
              })
              .catch(() => {
                weakSelf.setState(newState);
              });
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
            newState.isLoggedIn = false;
            weakSelf.setState(newState);
        });
    }

    private createSymmetricKeysAndCheckToken() {
        const newState = new MainState();
        newState.isLoggedIn = false;
        const weakSelf = this;

        AppCryptography.Instance.getSymmetricKeys()
        .then((symmetricKeys) => {
          weakSelf.appServiceHeaderInterceptor.setSymmetricKeys(
            symmetricKeys.symmetricKey,
            symmetricKeys.symmetricIV
          );
          weakSelf.checkToken();
        })
        .catch(() => {
          weakSelf.indicatorPresenter.dismiss();
          weakSelf.setState(newState);
        });
    }

    private checkToken() {
      const cookieAuthentication = process.env.REACT_APP_COOKIE_AUTHENTICATION;
      let userToken: string | null = null;

      if (cookieAuthentication !== "true") {
        userToken = this.storage.stringForKey(UICommonConstants.userTokenStorageKey);
      }

      const newState = new MainState();
      newState.isLoggedIn = false;

      this.indicatorPresenter.present();

      const checkTokenRequest = new CheckTokenRequestModel();
      checkTokenRequest.Token = userToken;

      const requestPath = `${process.env.REACT_APP_BACKOFFICE_AUTHENTICATION_CONTROLLER_NAME}/CheckToken`;
      const weakSelf = this;

      this.service.post(requestPath, checkTokenRequest, function (response: CheckTokenResponseModel) {
        weakSelf.indicatorPresenter.dismiss();

        if (response.status?.code !== 200) {
          weakSelf.setState(newState);
          return;
        }

        if (response.userRole != null) {
          weakSelf.appContext.setNumberForKey(BOCommonConstants.userRoleStorageKey, response.userRole);
        }

        weakSelf.decryptResponseAndUpdateState(response.userName ?? "");
      }, function (error: string) {
        weakSelf.handleServiceError("", error);
        newState.isLoggedIn = false;
        weakSelf.setState(newState);
      });
    }

    private decryptResponseAndUpdateState(encryptedUserName: string) {
        const newState = new MainState();
        newState.isLoggedIn = false;
        const weakSelf = this;

        AppCryptography.Instance.decrypt(encryptedUserName)
          .then((decrypted) => {
            weakSelf.storage.setStringForKey(
              BOCommonConstants.userNameStorageKey,
              decrypted
            );
            newState.isLoggedIn = true;
            weakSelf.setState(newState, () => {
                if (window.location.hash === "#!userChangePassword") {
                    weakSelf.updateLocation("#!userChangePassword");
                } else {
                    weakSelf.setState(newState, () => {
                        weakSelf.updateLocation(window.location.hash);
                    });
                }
            });
          })
          .catch(() => {
            weakSelf.indicatorPresenter.dismiss();
            weakSelf.setState(newState);
          });
    }

    private handleLoginSuccess() {
        const newState = new MainState();
        newState.isLoggedIn = true;

        this.setState(newState);
    }

    render() {
        if (this.state.isLoggedIn) {
            const userName = this.storage.stringForKey(BOCommonConstants.userNameStorageKey);

            return (
                <React.StrictMode>
                    <HeaderView userName={userName} />
                    <MenuController
                    controllerName={process.env.REACT_APP_BACKOFFICE_MENU_CONTROLLER_NAME} 
                    pageHash={this.state.pageHash ?? "dashboard"}
                    userName={userName} />
                    <div className="content-wrapper bg-body-secondary z-1 pt-2">
                        <NavigationView pageHash={this.state.pageHash ?? "dashboard"} />
                        <SelectionWrapperView pageHash={this.state.pageHash ?? "dashboard"}
                        selectionHash={this.state.selectionHash} />
                        <FooterView />
                    </div>
                </React.StrictMode>
            );
        }
        
        return (
            <React.StrictMode>
                <nav className="navbar navbar-expand-lg bg-body-tertiary z-3">
                    <div className="container-fluid">
                        <a className="navbar-brand" href={process.env.REACT_APP_BACKOFFICE_PAGE_URL}>
                            <h1 className="fs-5">{process.env.REACT_APP_APP_NAME}</h1>
                        </a>
                    </div>
                </nav>
                <nav className="navbar navbar-dark bg-dark bg-gradient flex-column align-items-start d-flex p-4 position-absolute start-0 bottom-0 z-2 overflow-visible sidebar" data-bs-theme="dark">
                </nav>
                <div className="content-wrapper bg-body-secondary z-1 pt-2">
                    <LoginController controllerName={process.env.REACT_APP_BACKOFFICE_AUTHENTICATION_CONTROLLER_NAME}
                    loginSuccessHandler={this.handleLoginSuccess} />
                    <FooterView />
                </div>
            </React.StrictMode>
        );
    }
}

export default Main;