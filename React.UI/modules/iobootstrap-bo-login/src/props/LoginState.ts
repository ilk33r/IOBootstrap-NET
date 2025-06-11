class LoginState {

    userName: string = "";
    password: string | null = null;
    captchaID: string | null = null;
    captcha: string | null = null;
    errorMessage: string = "";
}

export default LoginState;
