class MainState {

    isLoggedIn: boolean;
    pageHash: string | null;
    selectionHash: string | null;

    constructor() {
        this.isLoggedIn = false;
        this.pageHash = null;
        this.selectionHash = null;
    }
}

export default MainState;