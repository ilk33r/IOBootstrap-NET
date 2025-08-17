import MainProps from '../props/MainProps';
import MainState from '../props/MainState';
import NavigationView from '../../shared/views/NavigationView';
import React from 'react';
import { AppServiceHeaderAuthenticationInterceptor, Controller, DIHooks } from 'iobootstrap-ui-base';
import { Container } from 'react-bootstrap';

class Main extends Controller<MainProps, MainState> {

    private appServiceHeaderInterceptor: AppServiceHeaderAuthenticationInterceptor;

    constructor(props: MainProps) {
        super(props);

        this.state = new MainState();
        this.appServiceHeaderInterceptor = DIHooks.Instance.singletonForKey("appServiceHeaderInterceptor");

        this.service.baseUrl = (process.env.REACT_APP_API_URL === undefined) ? "" : process.env.REACT_APP_API_URL;
        this.service.appServiceHeaderInterceptor = DIHooks.Instance.singletonForKey("appServiceHeaderInterceptor");

        const authorization = (process.env.REACT_APP_AUTHORIZATION === undefined) ? "" : process.env.REACT_APP_AUTHORIZATION;
        this.appServiceHeaderInterceptor.initialize(authorization);
    }

    public componentDidMount?(): void {
        const weakSelf = this;
        window.addEventListener("navigation", function () {
            weakSelf.updateLocation(window.location.pathname);
        });

        window.addEventListener("popstate", function () {
            weakSelf.updateLocation(window.location.pathname);
        });

        window.addEventListener("onpushstate", function () {
            weakSelf.updateLocation(window.location.pathname);
        });

        this.updateLocation(window.location.pathname);
    }

    private updateLocation(path: string) {
        const cleanPath = path.substring(1, path.length).toLowerCase();

        const newState = new MainState();
        newState.isLoggedIn = this.state.isLoggedIn;
        newState.pagePath = cleanPath;

        if (cleanPath.length > 2) {
            newState.pathComponents = cleanPath.split("/");
        }

        this.setState(newState);
    }

    render() {
        return (
          <React.StrictMode>
            <Container fluid="md">
                <NavigationView
                    pagePath={this.state.pagePath ?? "dashboard"}
                    pathComponents={this.state.pathComponents}
                />
            </Container>
          </React.StrictMode>
        );
    }
}

export default Main;