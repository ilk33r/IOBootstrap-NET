import React from 'react';
import ReactDOM from 'react-dom';
import Main from './screens/main/controllers/Main';
import reportWebVitals from './reportWebVitals';
import 'jquery/src/jquery'
import 'bootstrap/dist/css/bootstrap.css';
import '@fortawesome/fontawesome-free/css/regular.css';
import '@fortawesome/fontawesome-free/css/all.css';
import './presentation/styles/AdminLTE.css';
import './presentation/styles/AdminSkins.css';
import './presentation/styles/App.css';
import 'bootstrap/dist/js/bootstrap.js'
import { CalloutView, IndicatorView, UploadModalView } from 'iobootstrap-bo-base';
import DIUserRoleHooks from './di/DIUserRoleHooks';

DIUserRoleHooks.setup();

let calloutViewRef = React.createRef<CalloutView>();
const calloutView = (<CalloutView ref={calloutViewRef} />);
ReactDOM.render(calloutView, document.getElementById('calloutWrapper'));

let indicatorViewRef = React.createRef<IndicatorView>();
const indicatorView = (<IndicatorView ref={indicatorViewRef} />);
ReactDOM.render(indicatorView, document.getElementById('indicatorWrapper'));

let uploadModalViewRef = React.createRef<UploadModalView>();
let uploadModalViewPresentHandler = function() {
  $('#uploadModal').modal({
    backdrop: 'static',
    keyboard: false
  });
};
let uploadModalViewDismissHandler = function() {
  $('#uploadModal').modal('hide');
};
const uploadModalView = (<UploadModalView ref={uploadModalViewRef}
  presentHandler={uploadModalViewPresentHandler}
  dismissHandler={uploadModalViewDismissHandler} />);

ReactDOM.render(uploadModalView, document.getElementById('uploadModalWrapper'));

const mainView = (<Main apiURL={process.env.REACT_APP_API_URL}
  authorization={process.env.REACT_APP_AUTHORIZATION}
  clientID={process.env.REACT_APP_BACKOFFICE_CLIENI_ID}
  clientSecret={process.env.REACT_APP_BACKOFFICE_CLIENI_SECRET} 
  calloutView={calloutViewRef}
  indicatorView={indicatorViewRef}
  uploadModalView={uploadModalViewRef} />);

ReactDOM.render(mainView, document.getElementById('pagecontent'));

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
