import CalloutPresenter from '../../../presentation/presenters/CalloutPresenter';
import CalloutViewPresenter from '../../../presentation/inerfaces/CalloutViewPresenter';
import CheckTokenRequestModel from '../models/CheckTokenRequestModel';
import CheckTokenResponseModel from '../models/CheckTokenResponseModel';
import CommonConstants from '../../../common/constants/CommonConstants';
import Controller from '../../../presentation/controllers/Controller';
import Footer from '../../shared/views/Footer';
import Header from '../../shared/views/Header';
import IndicatorPresenter from '../../../presentation/presenters/IndicatorPresenter';
import IndicatorViewPresenter from '../../../presentation/inerfaces/IndicatorViewPresenter';
import Login from '../../login/controllers/Login';
import MainProps from '../props/MainProps';
import MainState from '../props/MainState';
import Menu from '../../shared/controllers/Menu';
import NavigationView from '../../shared/views/NavigationView';
import React from 'react';

class Main extends Controller<MainProps, MainState> {

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

        this.handleLoginSuccess = this.handleLoginSuccess.bind(this);
    }

    public componentDidMount?(): void {
        if (window.location.hash === "#!usersLogout") {
            window.location.hash = "#!dashboard";
        }
        
        const userToken = this.storage.stringForKey(CommonConstants.userTokenStorageKey);
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
                    weakSelf.appContext.setNumberForKey(CommonConstants.userRoleStorageKey, response.userRole);
                }

                weakSelf.storage.setStringForKey(CommonConstants.userNameStorageKey, response.userName ?? "");
                    
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
            const userName = this.storage.stringForKey(CommonConstants.userNameStorageKey);

            return (
                <React.StrictMode>
                    <Header userName={userName} />
                    <Menu userName={userName}
                    controllerName={process.env.REACT_APP_BACKOFFICE_MENU_CONTROLLER_NAME} />
                    <NavigationView />
                    <Footer />
                </React.StrictMode>
            );
        }
        
        return (
            <React.StrictMode>
                <Login controllerName={process.env.REACT_APP_BACKOFFICE_AUTHENTICATION_CONTROLLER_NAME}
                 loginSuccessHandler={this.handleLoginSuccess} />
                <Footer />
            </React.StrictMode>
        );
    }
}

export default Main;