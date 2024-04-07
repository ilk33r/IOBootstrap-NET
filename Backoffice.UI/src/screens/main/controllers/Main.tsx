import CheckTokenRequestModel from '../models/CheckTokenRequestModel';
import CheckTokenResponseModel from '../models/CheckTokenResponseModel';
import MainProps from '../props/MainProps';
import MainState from '../props/MainState';
import NavigationView from '../../shared/views/NavigationView';
import SelectionWrapperView from '../../shared/views/SelectionWrapperView';
import React from 'react';
import { CalloutPresenter, CalloutViewPresenter, Controller, IndicatorPresenter, IndicatorViewPresenter, UICommonConstants, UploadModalPresenter, UploadModalViewPresenter } from 'iobootstrap-ui-base';
import { BOCommonConstants, BOController, FooterView, HeaderView } from 'iobootstrap-bo-base';
import { MenuController } from 'iobootstrap-bo-menu';
import { LoginController } from 'iobootstrap-bo-login';

class Main extends BOController<MainProps, MainState> {

    constructor(props: MainProps) {
        super(props);

        this.state = new MainState();

        this.service.baseUrl = (props.apiURL === undefined) ? "" : props.apiURL;
        this.service.authorization = (props.authorization === undefined) ? "" : props.authorization;
        this.service.clientID = (props.clientID === undefined) ? "" : props.clientID;
        this.service.clientSecret = (props.clientSecret === undefined) ? "" : props.clientSecret;
        
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
            hashName = hash.substr(2, hash.length);;

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
        
        const userToken = this.storage.stringForKey(UICommonConstants.userTokenStorageKey);
        const newState = new MainState();
        newState.isLoggedIn = false;

        if (userToken != null) {
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

                weakSelf.storage.setStringForKey(BOCommonConstants.userNameStorageKey, response.userName ?? "");
                    
                newState.isLoggedIn = true;       
                weakSelf.setState(newState);
            }, function (error: string) {
                weakSelf.handleServiceError("", error);
                newState.isLoggedIn = false;
                weakSelf.setState(newState);
            });

            return
        }

        this.setState(newState);
    }

    handleLoginSuccess() {
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
                    <MenuController userName={userName}
                    controllerName={process.env.REACT_APP_BACKOFFICE_MENU_CONTROLLER_NAME} />
                    <NavigationView pageHash={this.state.pageHash ?? "dashboard"} />
                    <SelectionWrapperView pageHash={this.state.pageHash ?? "dashboard"}
                    selectionHash={this.state.selectionHash} />
                    <FooterView />
                </React.StrictMode>
            );
        }
        
        return (
            <React.StrictMode>
                <LoginController controllerName={process.env.REACT_APP_BACKOFFICE_AUTHENTICATION_CONTROLLER_NAME}
                 loginSuccessHandler={this.handleLoginSuccess} />
                <FooterView />
            </React.StrictMode>
        );
    }
}

export default Main;