import { createRoot } from 'react-dom/client';
import React from 'react';
import Main from './screens/main/controllers/Main';
import reportWebVitals from './reportWebVitals';
import 'jquery/src/jquery'
import '@fortawesome/fontawesome-free/css/regular.css';
import '@fortawesome/fontawesome-free/css/solid.css';
import '@fortawesome/fontawesome-free/css/all.css';
import './presentation/styles/Variables.scss';
import './presentation/styles/Style.scss';
import './presentation/styles/Sidebar.scss';
import './presentation/styles/Selection.scss';
import './presentation/styles/Callout.scss';
import './presentation/styles/Indicator.scss';
import './presentation/styles/Dashboard.scss';
import './presentation/styles/App.scss';
import { CalloutView, IndicatorView, UploadModalView } from 'iobootstrap-bo-base';
import DIUserRoleHooks from './di/DIUserRoleHooks';
import DIControllerHooks from './di/DIControllerHooks';
import * as Bootstrap from 'bootstrap';

DIUserRoleHooks.setup();
DIControllerHooks.setup();

let calloutViewRef = React.createRef<CalloutView>();
const calloutView = (<CalloutView ref={calloutViewRef} />);
const calloutWrapperContainer = document.getElementById('calloutWrapper');
const calloutWrapperRoot = createRoot(calloutWrapperContainer!);
calloutWrapperRoot.render(calloutView);

let indicatorViewRef = React.createRef<IndicatorView>();
const indicatorView = (<IndicatorView ref={indicatorViewRef} />);
const indicatorViewContainer = document.getElementById('indicatorWrapper');
const indicatorViewRoot = createRoot(indicatorViewContainer!);
indicatorViewRoot.render(indicatorView);

let bsUploadModal: Bootstrap.Modal | null = null;
let uploadModalViewRef = React.createRef<UploadModalView>();
let uploadModalViewPresentHandler = function() {
    bsUploadModal = new Bootstrap.Modal('#uploadModal', {
        backdrop: 'static',
        keyboard: false
    });
    bsUploadModal?.show();
};
let uploadModalViewDismissHandler = function() {
    bsUploadModal?.hide();
};

const uploadModalView = (<UploadModalView ref={uploadModalViewRef}
    presentHandler={uploadModalViewPresentHandler}
    dismissHandler={uploadModalViewDismissHandler} />);
const uploadModalViewContainer = document.getElementById('uploadModalWrapper');
const uploadModalViewRoot = createRoot(uploadModalViewContainer!);
uploadModalViewRoot.render(uploadModalView);

const mainView = (<Main calloutView={calloutViewRef}
    indicatorView={indicatorViewRef}
    uploadModalView={uploadModalViewRef} />);
const mainViewContainer = document.getElementById('pagecontent');
const mainViewRoot = createRoot(mainViewContainer!);
mainViewRoot.render(mainView);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
