class MainState {

    isLoggedIn: boolean;
    pagePath: string | null;
    pathComponents: string[] | null;

    constructor() {
        this.isLoggedIn = false;
        this.pagePath = null;
        this.pathComponents = null;
    }
}

export default MainState;