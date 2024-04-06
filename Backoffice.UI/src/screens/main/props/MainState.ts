class MainState {

    isLoggedIn: boolean;
    isSelection: boolean;
    pageHash: string | null;

    constructor() {
        this.isLoggedIn = false;
        this.isSelection = false;
        this.pageHash = null;
    }
}

export default MainState;