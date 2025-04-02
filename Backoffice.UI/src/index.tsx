import { createRoot } from 'react-dom/client';
import React from 'react';
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
import { CalloutView, IndicatorView, ModalInputView, UploadModalView } from 'iobootstrap-bo-base';
import DIUserRoleHooks from './di/DIUserRoleHooks';
import DIControllerHooks from './di/DIControllerHooks';

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

let modalInputViewRef = React.createRef<ModalInputView>();
let modalInputViewPresentHandler = function() {
  $('#inputModal').modal({
    backdrop: 'static',
    keyboard: false
  });
};
let modalInputViewDismissHandler = function() {
  $('#inputModal').modal('hide');
};
const modalInputView = (<ModalInputView ref={modalInputViewRef}
  presentHandler={modalInputViewPresentHandler}
  dismissHandler={modalInputViewDismissHandler} />);
ReactDOM.render(modalInputView, document.getElementById('modalInputWrapper'));

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
const uploadModalViewContainer = document.getElementById('uploadModalWrapper');
const uploadModalViewRoot = createRoot(uploadModalViewContainer!);
uploadModalViewRoot.render(uploadModalView);

const mainView = (<Main calloutView={calloutViewRef}
  indicatorView={indicatorViewRef}
  modalInputView={modalInputViewRef}
  uploadModalView={uploadModalViewRef} />);
const mainViewContainer = document.getElementById('pagecontent');
const mainViewRoot = createRoot(mainViewContainer!);
mainViewRoot.render(mainView);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
