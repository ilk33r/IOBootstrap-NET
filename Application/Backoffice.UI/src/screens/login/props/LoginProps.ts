type LoginSuccessHandler = () => void;

interface LoginProps {

    controllerName: string | undefined;
    loginSuccessHandler: LoginSuccessHandler;
}

export default LoginProps;