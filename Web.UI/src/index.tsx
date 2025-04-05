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
import DIControllerHooks from './di/DIControllerHooks';

DIControllerHooks.setup();

const mainView = (<Main />);
const mainViewContainer = document.getElementById('pagecontent');
const mainViewRoot = createRoot(mainViewContainer!);
mainViewRoot.render(mainView);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
